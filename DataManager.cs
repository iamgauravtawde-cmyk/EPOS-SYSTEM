// ============================================================================
// FILE: DataManager.cs
// PURPOSE: Central data management system - Handles all business logic and file I/O
// AUTHOR: [Your Name]
// DATE: 19/11/2025
// ASSIGNMENT: A4 - EPOS System for Pop-Up Shop
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CozyWinterKnitsEPOS
{
    /// <summary>
    /// Static class managing all application data including products, transactions, and file operations
    /// Acts as central repository for business logic and data persistence
    /// Uses only List collections as per assignment requirements (no Dictionary/HashSet)
    /// </summary>
    public static class DataManager
    {
        #region Constants

        /// <summary>
        /// Stock level threshold for critical alerts (RED indicator)
        /// Items with stock below this trigger urgent reorder warnings
        /// </summary>
        private const int CRITICAL_STOCK_THRESHOLD = 5;

        /// <summary>
        /// Stock level threshold for low stock warnings (YELLOW indicator)
        /// Items with stock below this show cautionary alerts
        /// </summary>
        private const int LOW_STOCK_THRESHOLD = 10;

        /// <summary>
        /// Minimum number of product types required by assignment
        /// Must have at least 10 different product styles/types
        /// </summary>
        private const int MINIMUM_PRODUCT_TYPES = 10;

        /// <summary>
        /// Minimum number of size variants per product type
        /// Each product type must have 5 or more sizes
        /// </summary>
        private const int MINIMUM_SIZE_VARIANTS = 5;

        #endregion

        #region Data Storage (In-Memory Collections)

        /// <summary>
        /// Master product catalog containing all available SKUs
        /// Initialized at application startup with 50+ unique product-size combinations
        /// Assignment Requirement: 10 types × 5 sizes minimum ✓
        /// </summary>
        public static List<Product> ProductCatalog { get; set; } = new List<Product>();

        /// <summary>
        /// Collection of all completed transactions for current session
        /// Used for sales reports, analytics, and search functionality
        /// Persisted to sales_history.txt after each transaction
        /// </summary>
        public static List<Transaction> CompletedTransactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// Sequential counter for generating unique transaction identifiers
        /// Increments after each successful checkout (0001, 0002, 0003, etc.)
        /// </summary>
        public static int TransactionSequenceCounter { get; set; } = 1;

        /// <summary>
        /// Name of currently logged-in employee or cashier
        /// Set by LoginForm, displayed in MainForm, included in transaction records
        /// Bespoke Feature: Employee tracking system ✓
        /// </summary>
        public static string CurrentLoggedInEmployee { get; set; } = "System";

        #endregion

        #region File Paths (Assignment Requirement: Debug Folder)

        /// <summary>
        /// Full path to stock inventory file in debug/release folder
        /// Format: SKU,Category,ProductName,Size,Price,Stock
        /// Loaded at startup, saved at exit
        /// Assignment Requirement: Load from text file ✓
        /// </summary>
        private static readonly string StockInventoryFilePath =
            Path.Combine(Application.StartupPath, "stock.txt");

        /// <summary>
        /// Full path to sales history file in debug/release folder
        /// Transactions appended after each checkout
        /// Used for transaction search and reporting
        /// </summary>
        private static readonly string SalesHistoryFilePath =
            Path.Combine(Application.StartupPath, "sales_history.txt");

        #endregion

        // ============================================================================
        // PRODUCT INITIALIZATION METHODS
        // Assignment Requirement: 10+ product types, 5+ sizes each
        // ============================================================================

        #region Product Initialization

        /// <summary>
        /// Initialize complete product catalog with all 50+ SKUs
        /// Creates 10 product types with 5 size variations each
        /// Called once at application startup from Program.cs
        /// Assignment Requirement: Minimum 10 types × 5 sizes = 50+ SKUs ✓
        /// </summary>
        public static void InitializeProductCatalog()
        {
            ProductCatalog.Clear(); // Ensure clean slate

            // CATEGORY 1: SCARVES (2 product types × 5 sizes = 10 SKUs)
            AddProductWithSizeRange("Scarves", "Wool Scarf",
                new[] { "S", "M", "L", "XL", "XXL" },
                25.00m,
                new[] { 15, 20, 18, 12, 8 });

            AddProductWithSizeRange("Scarves", "Cashmere Scarf",
                new[] { "S", "M", "L", "XL", "XXL" },
                55.00m,
                new[] { 8, 12, 10, 6, 4 });

            // CATEGORY 2: BEANIES (2 product types × 5 sizes = 10 SKUs)
            AddProductWithSizeRange("Beanies", "Classic Beanie",
                new[] { "Kids", "Teen", "Adult-S", "Adult-M", "Adult-L" },
                18.00m,
                new[] { 12, 15, 20, 18, 10 });

            AddProductWithSizeRange("Beanies", "Pom-Pom Beanie",
                new[] { "Kids", "Teen", "Adult-S", "Adult-M", "Adult-L" },
                22.00m,
                new[] { 10, 12, 8, 6, 4 });

            // CATEGORY 3: GLOVES (2 product types × 5 sizes = 10 SKUs)
            AddProductWithSizeRange("Gloves", "Fingerless Gloves",
                new[] { "XS", "S", "M", "L", "XL" },
                20.00m,
                new[] { 8, 12, 15, 10, 5 });

            AddProductWithSizeRange("Gloves", "Winter Gloves",
                new[] { "XS", "S", "M", "L", "XL" },
                28.00m,
                new[] { 6, 10, 12, 8, 3 });

            // CATEGORY 4: SOCKS (2 product types × 5 sizes = 10 SKUs)
            AddProductWithSizeRange("Socks", "Thermal Socks",
                new[] { "S", "M", "L", "XL", "XXL" },
                12.00m,
                new[] { 20, 25, 22, 15, 10 });

            AddProductWithSizeRange("Socks", "Cozy Boot Socks",
                new[] { "S", "M", "L", "XL", "XXL" },
                15.00m,
                new[] { 15, 18, 16, 12, 7 });

            // CATEGORY 5: SWEATERS (2 product types × 5 sizes = 10 SKUs)
            AddProductWithSizeRange("Sweaters", "Cable Knit Sweater",
                new[] { "XS", "S", "M", "L", "XL" },
                65.00m,
                new[] { 5, 8, 10, 7, 4 });

            AddProductWithSizeRange("Sweaters", "Turtleneck Sweater",
                new[] { "XS", "S", "M", "L", "XL" },
                58.00m,
                new[] { 6, 9, 11, 8, 5 });

            // TOTAL: 50 unique SKUs successfully created
            // 10 product types × 5 size variants = 50+ individual items ✓
        }

        /// <summary>
        /// Helper method to create multiple size variants for a single product type
        /// Automatically generates unique SKUs and applies progressive pricing
        /// Larger sizes receive incrementally higher prices (realistic pricing model)
        /// </summary>
        /// <param name="categoryName">Product category classification</param>
        /// <param name="productName">Base product name without size</param>
        /// <param name="sizeVariantArray">Array of size options for this product</param>
        /// <param name="basePriceAmount">Starting price for smallest size</param>
        /// <param name="stockLevelArray">Initial stock quantities for each size</param>
        private static void AddProductWithSizeRange(string categoryName, string productName,
            string[] sizeVariantArray, decimal basePriceAmount, int[] stockLevelArray)
        {
            // Iterate through each size variant and create individual Product object
            for (int sizeIndex = 0; sizeIndex < sizeVariantArray.Length; sizeIndex++)
            {
                // Generate unique SKU for this specific product-size combination
                string generatedSKU = GenerateStockKeepingUnitCode(categoryName, productName,
                    sizeVariantArray[sizeIndex]);

                // Progressive pricing: larger sizes cost $2 more per size increment
                // Example: S=$25, M=$27, L=$29, XL=$31, XXL=$33
                decimal adjustedPrice = basePriceAmount + (sizeIndex * 2);

                // Create new Product instance and add to catalog
                ProductCatalog.Add(new Product(
                    generatedSKU,
                    categoryName,
                    productName,
                    sizeVariantArray[sizeIndex],
                    adjustedPrice,
                    stockLevelArray[sizeIndex]
                ));
            }
        }

        /// <summary>
        /// Generate unique Stock Keeping Unit (SKU) code from product attributes
        /// Format: CategoryCode-ProductCode-SizeCode
        /// Example: SC-WS-M (Scarves - Wool Scarf - Medium)
        /// Ensures each product-size combination has unique identifier
        /// </summary>
        /// <param name="categoryName">Product category</param>
        /// <param name="productName">Product name</param>
        /// <param name="sizeVariant">Size specification</param>
        /// <returns>Formatted SKU string</returns>
        private static string GenerateStockKeepingUnitCode(string categoryName, string productName,
            string sizeVariant)
        {
            // Extract first 2 characters of category and convert to uppercase
            string categoryCode = categoryName.Substring(0, Math.Min(2, categoryName.Length)).ToUpper();

            // Extract first letter of each word in product name
            string[] productNameWords = productName.Split(' ');
            string productCode = productNameWords.Length > 1
                ? $"{productNameWords[0][0]}{productNameWords[1][0]}".ToUpper()
                : productName.Substring(0, Math.Min(2, productName.Length)).ToUpper();

            // Combine codes with hyphens: SC-WS-M
            return $"{categoryCode}-{productCode}-{sizeVariant}";
        }

        #endregion

        // ============================================================================
        // FILE INPUT/OUTPUT OPERATIONS
        // Assignment Requirement: Load from file at startup, save on exit
        // ============================================================================

        #region File I/O Operations

        /// <summary>
        /// Load stock inventory levels from stock.txt file in debug folder
        /// Updates CurrentStockLevel for each product based on saved data
        /// Called at application startup before MainForm opens
        /// Assignment Requirement: Load stock data from text file ✓
        /// </summary>
        public static void LoadStockInventoryFromFile()
        {
            try
            {
                // Check if stock file exists in debug folder
                if (!File.Exists(StockInventoryFilePath))
                {
                    // First run scenario - create initial stock file
                    MessageBox.Show(
                        "Stock inventory file not found.\nCreating initial stock file with default values...",
                        "First Run - Initialization",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    SaveStockInventoryToFile();
                    return;
                }

                // Read all lines from stock file
                string[] fileContentLines = File.ReadAllLines(StockInventoryFilePath);

                int updatedProductCount = 0;

                // Skip header line (index 0), process data lines
                for (int lineIndex = 1; lineIndex < fileContentLines.Length; lineIndex++)
                {
                    string[] dataFields = fileContentLines[lineIndex].Split(',');

                    // Validate line has all required fields
                    if (dataFields.Length >= 6)
                    {
                        string skuIdentifier = dataFields[0].Trim();
                        int savedStockLevel = int.Parse(dataFields[5].Trim());

                        // Find matching product in catalog by SKU
                        Product matchingProduct = ProductCatalog.FirstOrDefault(
                            product => product.StockKeepingUnit == skuIdentifier);

                        if (matchingProduct != null)
                        {
                            matchingProduct.CurrentStockLevel = savedStockLevel;
                            updatedProductCount++;
                        }
                    }
                }

                MessageBox.Show(
                    $"Stock inventory loaded successfully!\n\n" +
                    $"Products Updated: {updatedProductCount}\n" +
                    $"File Location: {StockInventoryFilePath}",
                    "Load Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception exceptionDetails)
            {
                MessageBox.Show(
                    $"Error loading stock inventory file:\n\n{exceptionDetails.Message}\n\n" +
                    $"Using default stock levels instead.",
                    "File Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Save current stock inventory levels to stock.txt file in debug folder
        /// Overwrites existing file with current stock data
        /// Called when user exits application or clicks Save button
        /// Assignment Requirement: Save closing stock to text file ✓
        /// </summary>
        public static void SaveStockInventoryToFile()
        {
            try
            {
                using (StreamWriter fileWriter = new StreamWriter(StockInventoryFilePath))
                {
                    // Write CSV header line
                    fileWriter.WriteLine("SKU,Category,ProductName,Size,Price,Stock");

                    // Write each product as comma-separated values
                    foreach (Product productItem in ProductCatalog)
                    {
                        fileWriter.WriteLine(
                            $"{productItem.StockKeepingUnit}," +
                            $"{productItem.CategoryName}," +
                            $"{productItem.ProductName}," +
                            $"{productItem.SizeVariant}," +
                            $"{productItem.UnitPrice:F2}," +
                            $"{productItem.CurrentStockLevel}");
                    }
                }

                MessageBox.Show(
                    $"Stock inventory saved successfully!\n\n" +
                    $"Products Saved: {ProductCatalog.Count}\n" +
                    $"File Location: {StockInventoryFilePath}",
                    "Save Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception exceptionDetails)
            {
                MessageBox.Show(
                    $"Error saving stock inventory file:\n\n{exceptionDetails.Message}",
                    "File Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Append completed transaction details to sales_history.txt file
        /// Maintains audit trail of all sales for reporting and compliance
        /// Called immediately after successful checkout
        /// </summary>
        /// <param name="completedTransaction">Transaction object to persist</param>
        public static void SaveTransactionToSalesHistory(Transaction completedTransaction)
        {
            try
            {
                // Append mode (true parameter) - don't overwrite existing transactions
                using (StreamWriter fileWriter = new StreamWriter(SalesHistoryFilePath, true))
                {
                    // Write transaction header with identifier
                    fileWriter.WriteLine($"========================================");
                    fileWriter.WriteLine($"Transaction ID: {completedTransaction.TransactionIdentifier}");
                    fileWriter.WriteLine($"Date/Time: {completedTransaction.FormattedTimestamp}");
                    fileWriter.WriteLine($"Employee/Customer: {completedTransaction.CustomerEmployeeName}");
                    fileWriter.WriteLine($"========================================");

                    // Write line items purchased
                    fileWriter.WriteLine("Items Purchased:");
                    foreach (CartItem lineItem in completedTransaction.LineItemsCollection)
                    {
                        fileWriter.WriteLine(
                            $"  • {lineItem.ProductReference.ProductName} " +
                            $"({lineItem.ProductReference.SizeVariant}) " +
                            $"× {lineItem.PurchaseQuantity} " +
                            $"@ ${lineItem.ProductReference.UnitPrice:F2} " +
                            $"= ${lineItem.LineItemSubtotal:F2}");
                    }

                    // Write financial summary
                    fileWriter.WriteLine($"----------------------------------------");
                    fileWriter.WriteLine($"Subtotal: ${completedTransaction.SubtotalBeforeDiscount:F2}");
                    fileWriter.WriteLine($"Discount: -${completedTransaction.DiscountAmountApplied:F2} " +
                        $"({completedTransaction.DiscountPercentageRate:F1}%)");
                    fileWriter.WriteLine($"TOTAL PAID: ${completedTransaction.FinalTotalAmount:F2}");
                    fileWriter.WriteLine($"Total Items: {completedTransaction.TotalItemQuantityCount}");
                    fileWriter.WriteLine(); // Blank line separator
                }

                // Add to in-memory collection for current session reporting
                CompletedTransactions.Add(completedTransaction);
                TransactionSequenceCounter++;
            }
            catch (Exception exceptionDetails)
            {
                MessageBox.Show(
                    $"Error saving transaction to sales history:\n\n{exceptionDetails.Message}",
                    "Transaction Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        // ============================================================================
        // BUSINESS LOGIC & REPORTING METHODS
        // ============================================================================

        #region Business Logic Methods

        /// <summary>
        /// Retrieve all products with stock levels below specified threshold
        /// Used for low stock alert displays on cashier screen and reports
        /// Assignment Requirement: Alert when stock levels low ✓
        /// </summary>
        /// <param name="thresholdLevel">Stock level considered "low" (default 5)</param>
        /// <returns>List of products requiring reorder</returns>
        public static List<Product> GetLowStockProductsList(int thresholdLevel = CRITICAL_STOCK_THRESHOLD)
        {
            List<Product> lowStockProducts = new List<Product>();

            foreach (Product productItem in ProductCatalog)
            {
                // Include items that are low but not completely out of stock
                if (productItem.CurrentStockLevel > 0 &&
                    productItem.CurrentStockLevel < thresholdLevel)
                {
                    lowStockProducts.Add(productItem);
                }
            }

            return lowStockProducts;
        }

        /// <summary>
        /// Search for specific transaction by its unique identifier
        /// Assignment Requirement: Search by Transaction ID ✓
        /// Used in SearchForm for transaction lookup
        /// </summary>
        /// <param name="transactionIdentifier">Transaction ID to find</param>
        /// <returns>Matching Transaction object or null if not found</returns>
        public static Transaction FindTransactionByIdentifier(string transactionIdentifier)
        {
            return CompletedTransactions.FirstOrDefault(
                transaction => transaction.TransactionIdentifier.Equals(
                    transactionIdentifier,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Search for all transactions completed on specific date
        /// Assignment Requirement: Search by Date ✓
        /// Used in SearchForm for daily transaction review
        /// </summary>
        /// <param name="searchDate">Date to search for transactions</param>
        /// <returns>List of all transactions from that date</returns>
        public static List<Transaction> FindTransactionsByDate(DateTime searchDate)
        {
            List<Transaction> matchingTransactions = new List<Transaction>();

            foreach (Transaction transactionItem in CompletedTransactions)
            {
                // Compare only date portion (ignore time)
                if (transactionItem.TransactionTimestamp.Date == searchDate.Date)
                {
                    matchingTransactions.Add(transactionItem);
                }
            }

            return matchingTransactions;
        }

        /// <summary>
        /// Calculate aggregate sales statistics for reporting period
        /// Returns tuple with total revenue, item count, and transaction count
        /// Used in SalesReportForm to display summary metrics
        /// </summary>
        /// <returns>Tuple: (TotalRevenue, TotalItemsSold, TransactionCount)</returns>
        public static (decimal TotalRevenue, int TotalItemsSold, int TransactionCount)
            CalculateSalesStatistics()
        {
            decimal aggregateRevenue = 0;
            int aggregateItemCount = 0;

            foreach (Transaction transactionItem in CompletedTransactions)
            {
                aggregateRevenue += transactionItem.FinalTotalAmount;
                aggregateItemCount += transactionItem.TotalItemQuantityCount;
            }

            return (aggregateRevenue, aggregateItemCount, CompletedTransactions.Count);
        }

        #endregion
    }
}
