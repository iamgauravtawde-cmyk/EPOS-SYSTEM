using System;

namespace CozyWinterKnitsEPOS
{
    /// <summary>
    /// Represents a line item in the shopping cart
    /// Links a Product reference with purchase quantity for current transaction
    /// Author: [Your Name]
    /// Date: 19/11/2025
    /// Assignment: A4 - EPOS System
    /// </summary>
    public class CartItem
    {
        #region Properties

        /// <summary>
        /// Reference to the Product being purchased
        /// Contains all product details (name, price, stock, category)
        /// </summary>
        public Product ProductReference { get; set; }

        /// <summary>
        /// Quantity of this product customer intends to purchase
        /// Must be validated against available stock before checkout
        /// </summary>
        public int PurchaseQuantity { get; set; }

        /// <summary>
        /// Calculated subtotal for this line item
        /// Formula: UnitPrice × PurchaseQuantity
        /// Example: $25.00 × 2 = $50.00
        /// </summary>
        public decimal LineItemSubtotal => ProductReference.UnitPrice * PurchaseQuantity;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new cart item with product and quantity
        /// </summary>
        /// <param name="productReference">Product to add to cart</param>
        /// <param name="purchaseQuantity">Number of units to purchase</param>
        public CartItem(Product productReference, int purchaseQuantity)
        {
            ProductReference = productReference;
            PurchaseQuantity = purchaseQuantity;
        }

        #endregion

        #region Display Properties for DataGridView Binding

        /// <summary>
        /// Formatted product name for DataGridView display
        /// Shows: "Wool Scarf (M)"
        /// </summary>
        public string DisplayProductName => ProductReference.FormattedDisplayName;

        /// <summary>
        /// Size variant for separate DataGridView column
        /// Shows: "M" or "Adult-L"
        /// </summary>
        public string DisplaySizeVariant => ProductReference.SizeVariant;

        /// <summary>
        /// Unit price for DataGridView column
        /// Shows: $25.00
        /// </summary>
        public decimal DisplayUnitPrice => ProductReference.UnitPrice;

        /// <summary>
        /// Category name for DataGridView column
        /// Shows: "Scarves"
        /// </summary>
        public string DisplayCategoryName => ProductReference.CategoryName;

        /// <summary>
        /// Formatted line total with currency symbol
        /// Shows: "$50.00"
        /// </summary>
        public string DisplayFormattedTotal => $"${LineItemSubtotal:F2}";

        #endregion

        #region Methods

        /// <summary>
        /// Returns string representation for debugging
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayProductName} x {PurchaseQuantity} = ${LineItemSubtotal:F2}";
        }

        #endregion
    }
}
