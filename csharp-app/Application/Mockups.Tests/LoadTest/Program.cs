using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace Mockups.LoadTests
{
    class Program
    {
        private static readonly string BaseUrl = Environment.GetEnvironmentVariable("LOAD_TEST_BASE_URL") ?? "http://localhost:5146";
        private static readonly string TestUserEmail = Environment.GetEnvironmentVariable("LOAD_TEST_USER_EMAIL") ?? "Java@DlyaLox.ov";
        private static readonly string TestUserPassword = Environment.GetEnvironmentVariable("LOAD_TEST_USER_PASSWORD") ?? ".NetDlyaPacan0v";

        // Metrics tracking
        private static int _browsingRequests = 0;
        private static int _browsingSuccesses = 0;
        private static int _browsingFailures = 0;
        private static long _browsingTotalLatency = 0;
        private static int _userJourneyRequests = 0;
        private static int _userJourneySuccesses = 0;
        private static int _userJourneyFailures = 0;
        private static long _userJourneyTotalLatency = 0;
        private static readonly object _lock = new object();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting load tests for the food ordering application...");
            Console.WriteLine($"Base URL: {BaseUrl}");

            // First, let's check if the application is responding
            Console.WriteLine("Checking if application is running...");
            bool isAppRunning = await CheckAppStatus();
            if (!isAppRunning)
            {
                Console.WriteLine("Application is not responding. Please make sure it's running on the specified URL.");
                return;
            }

            Console.WriteLine("Application is responding. Starting load tests...");
            Console.WriteLine($"Running tests for 30 seconds...");

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(30)); // Run for 30 seconds

            var tasks = new List<Task>
            {
                RunAnonymousBrowsingTest(cts.Token),
                RunUserJourneyTest(cts.Token)
            };

            var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Load test completed after 30 seconds.");
            }

            var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var totalDurationSeconds = (endTime - startTime) / 1000.0;

            Console.WriteLine("Load testing finished.");
            Console.WriteLine();
            Console.WriteLine("=== METRICS SUMMARY ===");
            PrintMetrics("Anonymous Browsing", _browsingRequests, _browsingSuccesses, _browsingFailures, _browsingTotalLatency, totalDurationSeconds);
            PrintMetrics("User Journey", _userJourneyRequests, _userJourneySuccesses, _userJourneyFailures, _userJourneyTotalLatency, totalDurationSeconds);
        }

        static void PrintMetrics(string testName, int totalRequests, int successes, int failures, long totalLatency, double durationSeconds)
        {
            Console.WriteLine($"{testName}:");
            Console.WriteLine($"  Total Requests: {totalRequests}");
            Console.WriteLine($"  Successes: {successes}");
            Console.WriteLine($"  Failures: {failures}");

            double rps = durationSeconds > 0 ? totalRequests / durationSeconds : 0;
            Console.WriteLine($"  RPS (Requests Per Second): {rps:F2}");

            double errorRate = totalRequests > 0 ? (double)failures / totalRequests * 100 : 0;
            Console.WriteLine($"  Error Rate: {errorRate:F2}%");

            double avgLatency = successes > 0 ? (double)totalLatency / successes : 0;
            Console.WriteLine($"  Average Latency: {avgLatency:F2}ms");

            Console.WriteLine();
        }

        static async Task<bool> CheckAppStatus()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.GetAsync(BaseUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        static async Task RunAnonymousBrowsingTest(CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            int requestCount = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    try
                    {
                        var response = await client.GetAsync($"{BaseUrl}/Menu");
                        var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var latency = endTime - startTime;

                        lock (_lock)
                        {
                            _browsingRequests++;
                            _browsingTotalLatency += latency;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            requestCount++;
                            Console.WriteLine($"[BROWSING] Request #{requestCount}: Status {response.StatusCode}, Latency: {latency}ms");
                            lock (_lock)
                            {
                                _browsingSuccesses++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[BROWSING] Request #{requestCount}: Failed with status {response.StatusCode}, Latency: {latency}ms");
                            lock (_lock)
                            {
                                _browsingFailures++;
                            }
                        }

                        // Add small delay between requests to make it more realistic
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        var endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var latency = endTime - startTime;

                        Console.WriteLine($"[BROWSING] Error: {ex.Message}, Latency: {latency}ms");
                        lock (_lock)
                        {
                            _browsingRequests++;
                            _browsingFailures++;
                            _browsingTotalLatency += latency;
                        }

                        await Task.Delay(1000, cancellationToken); // Wait before retrying
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation token is triggered
            }

            Console.WriteLine($"Anonymous browsing test completed. Total requests: {requestCount}");
        }

        static async Task RunUserJourneyTest(CancellationToken cancellationToken)
        {
            // For this test to work, a user would need to exist in the database
            // The test will try to log in and perform the full journey
            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer() // To maintain session
            };
            var client = new HttpClient(handler);
            int requestCount = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var journeyStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                    try
                    {
                        // Step 1: Get the login page first to retrieve the anti-forgery token
                        var loginPageStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var loginPageResponse = await client.GetAsync($"{BaseUrl}/Account/Login");
                        var loginPageEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var loginPageLatency = loginPageEndTime - loginPageStartTime;

                        string loginPageContent = "";

                        lock (_lock)
                        {
                            _userJourneyRequests++;
                            _userJourneyTotalLatency += loginPageLatency;
                        }

                        if (loginPageResponse.IsSuccessStatusCode)
                        {
                            loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            lock (_lock)
                            {
                                _userJourneyFailures++;
                            }
                            Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Failed to get login page - {loginPageResponse.StatusCode}");
                        }

                        // Extract the anti-forgery token from the login page
                        string antiForgeryToken = ExtractAntiForgeryToken(loginPageContent);

                        // Step 2: Try to login with the anti-forgery token
                        var loginFormData = new Dictionary<string, string>
                        {
                            {"Email", TestUserEmail},
                            {"Password", TestUserPassword},
                            {"RememberMe", "false"},
                            {"__RequestVerificationToken", antiForgeryToken} // Add the anti-forgery token
                        };

                        var loginStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var content = new FormUrlEncodedContent(loginFormData);
                        var loginResponse = await client.PostAsync($"{BaseUrl}/Account/Login", content);
                        var loginEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var loginLatency = loginEndTime - loginStartTime;

                        lock (_lock)
                        {
                            _userJourneyRequests++;
                            _userJourneyTotalLatency += loginLatency;
                        }

                        if (loginResponse.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Login successful, Latency: {loginLatency}ms");
                            lock (_lock)
                            {
                                _userJourneySuccesses++;
                            }

                            // Step 2: Get menu to find first item
                            var menuStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            var menuResponse = await client.GetAsync($"{BaseUrl}/Menu");
                            var menuEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            var menuLatency = menuEndTime - menuStartTime;

                            lock (_lock)
                            {
                                _userJourneyRequests++;
                                _userJourneyTotalLatency += menuLatency;
                            }

                            if (menuResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Menu retrieval successful, Latency: {menuLatency}ms");

                                string menuContent = await menuResponse.Content.ReadAsStringAsync();
                                string firstMenuItemId = ExtractFirstMenuItemId(menuContent);

                                if (!string.IsNullOrEmpty(firstMenuItemId))
                                {
                                    // Step 3: Add first menu item to cart
                                    var addToCartStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                    string addToCartPageContent = await client.GetStringAsync($"{BaseUrl}/Menu/AddToCart?id={firstMenuItemId}");
                                    string cartAntiForgeryToken = ExtractAntiForgeryToken(addToCartPageContent);

                                    var addToCartData = new Dictionary<string, string>
                                    {
                                        {"id", firstMenuItemId},
                                        {"amount", "1"},
                                        {"__RequestVerificationToken", cartAntiForgeryToken}
                                    };

                                    var addToCartContent = new FormUrlEncodedContent(addToCartData);
                                    var addToCartResponse = await client.PostAsync($"{BaseUrl}/Menu/AddToCart", addToCartContent);
                                    var addToCartEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                    var addToCartLatency = addToCartEndTime - addToCartStartTime;

                                    lock (_lock)
                                    {
                                        _userJourneyRequests++;
                                        _userJourneyTotalLatency += addToCartLatency;
                                    }

                                    if (addToCartResponse.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Added item to cart successfully, Latency: {addToCartLatency}ms");
                                        lock (_lock)
                                        {
                                            _userJourneySuccesses++;
                                        }

                                        // Step 4: Access cart
                                        var cartStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                        var cartResponse = await client.GetAsync($"{BaseUrl}/Cart");
                                        var cartEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                        var cartLatency = cartEndTime - cartStartTime;

                                        lock (_lock)
                                        {
                                            _userJourneyRequests++;
                                            _userJourneyTotalLatency += cartLatency;
                                        }

                                        Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Cart access: {cartResponse.StatusCode}, Latency: {cartLatency}ms");

                                        if (cartResponse.IsSuccessStatusCode)
                                        {
                                            // Step 4.5: Add address if user doesn't have one yet
                                            var addressesResponse = await client.GetAsync($"{BaseUrl}/Account");
                                            if (addressesResponse.IsSuccessStatusCode)
                                            {
                                                string addressesPageContent = await addressesResponse.Content.ReadAsStringAsync();
                                                // Check if user already has an address by looking for address-related elements more reliably
                                                if (!addressesPageContent.Contains("AddAddress") || !addressesPageContent.Contains("адрес") || addressesPageContent.Contains("Адреса не найдены"))
                                                {
                                                    // Get the anti-forgery token from the add address page
                                                    var addAddressPageResponse = await client.GetAsync($"{BaseUrl}/Account/AddAddress");
                                                    if (addAddressPageResponse.IsSuccessStatusCode)
                                                    {
                                                        string addAddressPageContent = await addAddressPageResponse.Content.ReadAsStringAsync();
                                                        string addressAntiForgeryToken = ExtractAntiForgeryToken(addAddressPageContent);

                                                        var addressStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                        var addressFormData = new Dictionary<string, string>
                                                        {
                                                            {"Name", "Тестовый адрес"},
                                                            {"StreetName", "Тестовая улица"},
                                                            {"HouseNumber", "1"},
                                                            {"FlatNumber", "101"},
                                                            {"EntranceNumber", "1"},
                                                            {"Note", "Тестовый адрес для нагрузочного теста"},
                                                            {"IsMainAddress", "true"},
                                                            {"__RequestVerificationToken", addressAntiForgeryToken}
                                                        };

                                                        var addressContent = new FormUrlEncodedContent(addressFormData);
                                                        var addressResponse = await client.PostAsync($"{BaseUrl}/Account/AddAddress", addressContent);
                                                        var addressEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                                        var addressLatency = addressEndTime - addressStartTime;

                                                        Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Address creation: {addressResponse.StatusCode}, Latency: {addressLatency}ms");

                                                        // Wait briefly to ensure the address is saved before proceeding to order creation
                                                        await Task.Delay(500, cancellationToken);
                                                    }
                                                }
                                            }

                                            // Step 5: Place order
                                            // First, get the order creation page to extract the anti-forgery token
                                            var orderPageResponse = await client.GetAsync($"{BaseUrl}/Orders/Create");
                                            string orderPageContent = await orderPageResponse.Content.ReadAsStringAsync();
                                            string orderAntiForgeryToken = ExtractAntiForgeryToken(orderPageContent);

                                            var orderStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                            var orderFormData = new Dictionary<string, string>
                                            {
                                                {"PostModel.Address", "Тестовая улица, д. 1, кв. 101"},
                                                {"PostModel.DeliveryTime", DateTime.Now.AddHours(1).ToString("yyyy-MM-ddTHH:mm")}, // Set delivery time to 1 hour from now
                                                {"__RequestVerificationToken", orderAntiForgeryToken}
                                            };

                                            var orderContent = new FormUrlEncodedContent(orderFormData);
                                            var orderResponse = await client.PostAsync($"{BaseUrl}/Orders/Create", orderContent);
                                            var orderEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                            var orderLatency = orderEndTime - orderStartTime;

                                            lock (_lock)
                                            {
                                                _userJourneyRequests++;
                                                _userJourneyTotalLatency += orderLatency;
                                            }

                                            Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Order placement: {orderResponse.StatusCode}, Latency: {orderLatency}ms");

                                            if (orderResponse.IsSuccessStatusCode)
                                            {
                                                lock (_lock)
                                                {
                                                    _userJourneySuccesses++;
                                                }
                                            }
                                            else
                                            {
                                                lock (_lock)
                                                {
                                                    _userJourneyFailures++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Cart access failed - {cartResponse.StatusCode}");
                                            lock (_lock)
                                            {
                                                _userJourneyFailures++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Failed to add item to cart - {addToCartResponse.StatusCode}");
                                        lock (_lock)
                                        {
                                            _userJourneyFailures++;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: No menu items found");
                                    lock (_lock)
                                    {
                                        _userJourneyFailures++;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Menu retrieval failed - {menuResponse.StatusCode}");
                                lock (_lock)
                                {
                                    _userJourneyFailures++;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Login failed - {loginResponse.StatusCode}, Latency: {loginLatency}ms");
                            lock (_lock)
                            {
                                _userJourneyFailures++;
                            }
                        }

                        var journeyEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var totalJourneyLatency = journeyEndTime - journeyStartTime;
                        Console.WriteLine($"[USER-JOURNEY] #{requestCount + 1}: Total journey latency: {totalJourneyLatency}ms");

                        requestCount++;

                        // Add delay between user journeys to make it more realistic
                        await Task.Delay(5000, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        var journeyEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        var totalJourneyLatency = journeyEndTime - journeyStartTime;

                        Console.WriteLine($"[USER-JOURNEY] Error: {ex.Message}, Total journey latency: {totalJourneyLatency}ms");
                        lock (_lock)
                        {
                            _userJourneyRequests++;
                            _userJourneyFailures++;
                            _userJourneyTotalLatency += totalJourneyLatency;
                        }

                        await Task.Delay(5000, cancellationToken); // Wait before retrying
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation token is triggered
            }

            Console.WriteLine($"User journey test completed. Total journeys: {requestCount}");
        }

        static string ExtractAntiForgeryToken(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";

            try
            {
                var tokenStart = html.IndexOf("name=\"__RequestVerificationToken\"");
                if (tokenStart >= 0)
                {
                    var valueStart = html.IndexOf("value=\"", tokenStart);
                    if (valueStart >= 0)
                    {
                        valueStart += 7; // length of "value=\""
                        var valueEnd = html.IndexOf("\"", valueStart);
                        if (valueEnd > valueStart)
                        {
                            return html.Substring(valueStart, valueEnd - valueStart);
                        }
                    }
                }

                // Also try to find it in hidden input fields
                var hiddenInputStart = html.IndexOf("<input type=\"hidden\" name=\"__RequestVerificationToken\"");
                if (hiddenInputStart >= 0)
                {
                    var valueStart = html.IndexOf("value=\"", hiddenInputStart);
                    if (valueStart >= 0)
                    {
                        valueStart += 7;
                        var valueEnd = html.IndexOf("\"", valueStart);
                        if (valueEnd > valueStart)
                        {
                            return html.Substring(valueStart, valueEnd - valueStart);
                        }
                    }
                }

                return ""; // Return empty string if not found
            }
            catch
            {
                return ""; // Return empty string on error
            }
        }

        /// <summary>
        /// Extracts the first menu item ID from the HTML response
        /// </summary>
        private static string ExtractFirstMenuItemId(string html)
        {
            try
            {
                // Look for data-menu-item-id attribute (added for testing purposes)
                var dataAttrPattern = "data-menu-item-id=\"";
                var idx = html.IndexOf(dataAttrPattern);
                if (idx >= 0)
                {
                    idx += dataAttrPattern.Length;
                    var startIdx = idx;
                    while (idx < html.Length && html[idx] != '"' && html[idx] != '\'')
                    {
                        idx++;
                    }
                    var extractedId = html.Substring(startIdx, idx - startIdx);
                    if (!string.IsNullOrEmpty(extractedId) && Guid.TryParse(extractedId, out _))
                    {
                        return extractedId;
                    }
                }

                // Look for the first AddToCart link with an ID in href="/Menu/AddToCart?id=GUID"
                var addToCartPattern = "/Menu/AddToCart?id=";
                idx = html.IndexOf(addToCartPattern);
                if (idx >= 0)
                {
                    idx += addToCartPattern.Length;
                    var startIdx = idx;
                    // Look for the end of the GUID (up to next quote, space, or other delimiter)
                    while (idx < html.Length && html[idx] != '"' && html[idx] != '\'' &&
                           html[idx] != ' ' && html[idx] != '&' && html[idx] != '#' &&
                           html[idx] != '?' && html[idx] != '>' && html[idx] != '<')
                    {
                        idx++;
                    }
                    var extractedId = html.Substring(startIdx, idx - startIdx);
                    // Validate that it looks like a GUID
                    if (!string.IsNullOrEmpty(extractedId) && Guid.TryParse(extractedId, out _))
                    {
                        return extractedId;
                    }
                }

                // Also try alternative pattern - looking for asp-route-id attribute
                var routePattern = "asp-route-id=\"";
                idx = html.IndexOf(routePattern);
                if (idx >= 0)
                {
                    idx += routePattern.Length;
                    var startIdx = idx;
                    while (idx < html.Length && html[idx] != '"' && html[idx] != '\'')
                    {
                        idx++;
                    }
                    var extractedId = html.Substring(startIdx, idx - startIdx);
                    if (!string.IsNullOrEmpty(extractedId) && Guid.TryParse(extractedId, out _))
                    {
                        return extractedId;
                    }
                }

                // Look for the hidden span with data-menu-item-id class
                var hiddenSpanPattern = "<span class=\"data-menu-item-id\"";
                idx = html.IndexOf(hiddenSpanPattern);
                if (idx >= 0)
                {
                    var dataAttrPos = html.IndexOf("data-menu-item-id=\"", idx);
                    if (dataAttrPos >= 0)
                    {
                        dataAttrPos += "data-menu-item-id=\"".Length;
                        var startIdx = dataAttrPos;
                        while (dataAttrPos < html.Length && html[dataAttrPos] != '"' && html[dataAttrPos] != '\'')
                        {
                            dataAttrPos++;
                        }
                        var extractedId = html.Substring(startIdx, dataAttrPos - startIdx);
                        if (!string.IsNullOrEmpty(extractedId) && Guid.TryParse(extractedId, out _))
                        {
                            return extractedId;
                        }
                    }
                }

                return null; // No valid ID found
            }
            catch (Exception ex)
            {
                return null; // Return null instead of throwing to avoid test failures
            }
        }
    }
}