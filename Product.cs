using System;

namespace CozyWinterKnitsEPOS
{
    /// <summary>
    /// Represents a single product-size combination (SKU) in the inventory
    /// Each size variant is treated as a separate product with its own price and stock level
    /// Author: [Your Name]
    /// Date: 19/11/2025
    /// Assignment: A4 - EPOS System
    /// </summary>
    public class Product
    {
        #region Properties

        /// <summary>
        /// Unique Stock Keeping Unit identifier
        /// Format: CategoryCode-ProductCode-SizeCode (e.g., SC-WS-M)
        /// </summary>
        public string StockKeepingUnit { get; set; }

        /// <summary>
        /// Product category for grouping similar items
        /// Examples: Scarves, Beanies, Gloves, Socks, Sweaters
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Base product name without size specification
        /// Examples: "Wool Scarf", "Classic Beanie", "Winter Gloves"
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Size variant identifier for this product
        /// Examples: "S", "M", "L", "XL", "XXL", "Kids", "Teen", "Adult-M"
        /// </summary>
        public string SizeVariant { get; set; }

        /// <summary>
        /// Unit price for this specific product-size combination in currency
        /// Larger sizes typically have higher prices
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Current available stock quantity
        /// Decremented after each sale, used for validation
        /// </summary>
        public int CurrentStockLevel { get; set; }

        /// <summary>
        /// Original stock level at initialization
        /// Used to calculate units sold for reporting purposes
        /// </summary>
        public int InitialStockLevel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new Product instance with all required attributes
        /// </summary>
        /// <param name="stockKeepingUnit">Unique SKU identifier</param>
        /// <param name="categoryName">Product category</param>
        /// <param name="productName">Product name</param>
        /// <param name="sizeVariant">Size specification</param>
        /// <param name="unitPrice">Price per unit</param>
        /// <param name="stockLevel">Initial stock quantity</param>
        public Product(string stockKeepingUnit, string categoryName, string productName,
                      string sizeVariant, decimal unitPrice, int stockLevel)
        {
            StockKeepingUnit = stockKeepingUnit;
            CategoryName = categoryName;
            ProductName = productName;
            SizeVariant = sizeVariant;
            UnitPrice = unitPrice;
            CurrentStockLevel = stockLevel;
            InitialStockLevel = stockLevel;
        }

        #endregion

        #region Computed Properties

        /// <summary>
        /// Returns formatted product name with size for UI display
        /// Example: "Wool Scarf (M)" or "Classic Beanie (Adult-L)"
        /// </summary>
        public string FormattedDisplayName => $"{ProductName} ({SizeVariant})";

        /// <summary>
        /// Calculates current stock status for color-coding in UI
        /// Returns: "OUT_OF_STOCK", "CRITICAL", "LOW", or "GOOD"
        /// Used to apply visual indicators in DataGridView
        /// </summary>
        public string StockStatusIndicator
        {
            get
            {
                if (CurrentStockLevel == 0) return "OUT_OF_STOCK";
                if (CurrentStockLevel < 5) return "CRITICAL";
                if (CurrentStockLevel < 10) return "LOW";
                return "GOOD";
            }
        }

        /// <summary>
        /// Indicates whether product can be added to cart
        /// Returns false when stock is depleted
        /// </summary>
        public bool IsCurrentlyAvailable => CurrentStockLevel > 0;

        /// <summary>
        /// Calculates total units sold since initialization
        /// Used for sales reporting and analytics
        /// </summary>
        public int TotalUnitsSold => InitialStockLevel - CurrentStockLevel;

        #endregion

        #region Methods

        /// <summary>
        /// Returns string representation for debugging purposes
        /// </summary>
        public override string ToString()
        {
            return $"{StockKeepingUnit}: {FormattedDisplayName} - ${UnitPrice:F2} - Stock: {CurrentStockLevel} ({StockStatusIndicator})";
        }

        #endregion
    }
}

