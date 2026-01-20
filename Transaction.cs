using System;
using System.Collections.Generic;
using System.Linq;

namespace CozyWinterKnitsEPOS
{
    /// <summary>
    /// Represents a completed sales transaction with all relevant details
    /// Stores line items, pricing, discounts, customer information, and timestamp
    /// Persisted to sales_history.txt for reporting purposes
    /// Author: [Your Name]
    /// Date: 19/11/2025
    /// Assignment: A4 - EPOS System
    /// </summary>
    public class Transaction
    {
        #region Properties

        /// <summary>
        /// Unique transaction identifier
        /// Format: TXN-YYYYMMDD-#### (e.g., TXN-20251119-0001)
        /// Used for searching and audit trail
        /// </summary>
        public string TransactionIdentifier { get; set; }

        /// <summary>
        /// Timestamp when transaction was completed
        /// Used for daily reports and date-based searches
        /// </summary>
        public DateTime TransactionTimestamp { get; set; }

        /// <summary>
        /// Collection of line items purchased in this transaction
        /// Each CartItem contains Product reference and quantity
        /// Assignment Requirement: Multiple items per transaction ✓
        /// </summary>
        public List<CartItem> LineItemsCollection { get; set; }

        /// <summary>
        /// Total amount before any discounts applied
        /// Sum of all line item subtotals
        /// </summary>
        public decimal SubtotalBeforeDiscount { get; set; }

        /// <summary>
        /// Discount amount deducted from subtotal (in currency)
        /// Example: $7.60 discount on $76.00 subtotal
        /// </summary>
        public decimal DiscountAmountApplied { get; set; }

        /// <summary>
        /// Discount rate as percentage
        /// Example: 10 (representing 10%, not 0.10)
        /// </summary>
        public decimal DiscountPercentageRate { get; set; }

        /// <summary>
        /// Final amount customer pays after discount
        /// Formula: SubtotalBeforeDiscount - DiscountAmountApplied
        /// This is the transaction total
        /// </summary>
        public decimal FinalTotalAmount { get; set; }

        /// <summary>
        /// Total count of items sold (sum of all quantities)
        /// Example: 2 scarves + 1 beanie = 3 items
        /// </summary>
        public int TotalItemQuantityCount { get; set; }

        /// <summary>
        /// Customer or employee name
        /// Default: "Walk-in" for anonymous customers
        /// Can be set to logged-in employee name (bespoke feature)
        /// </summary>
        public string CustomerEmployeeName { get; set; }

        /// <summary>
        /// Promotional coupon code applied (if any)
        /// Part of bespoke discount feature
        /// Empty string if no coupon used
        /// </summary>
        public string PromotionalCouponCode { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new transaction with default values
        /// Sets current timestamp and initializes empty line items collection
        /// </summary>
        public Transaction()
        {
            LineItemsCollection = new List<CartItem>();
            TransactionTimestamp = DateTime.Now;
            CustomerEmployeeName = "Walk-in";
            PromotionalCouponCode = "";
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Generate unique transaction ID with date and counter
        /// Format: TXN-YYYYMMDD-#### where #### is zero-padded counter
        /// Example: TXN-20251119-0001, TXN-20251119-0002
        /// </summary>
        /// <param name="sequentialCounter">Transaction counter from DataManager</param>
        /// <returns>Formatted transaction identifier string</returns>
        public static string GenerateUniqueTransactionID(int sequentialCounter)
        {
            return $"TXN-{DateTime.Now:yyyyMMdd}-{sequentialCounter:D4}";
        }

        /// <summary>
        /// Calculate subtotal by summing all line item subtotals
        /// Should be called before setting SubtotalBeforeDiscount property
        /// </summary>
        public void CalculateSubtotalAmount()
        {
            SubtotalBeforeDiscount = LineItemsCollection.Sum(item => item.LineItemSubtotal);
        }

        /// <summary>
        /// Calculate total item quantity by summing all line item quantities
        /// Should be called before setting TotalItemQuantityCount property
        /// </summary>
        public void CalculateTotalItemCount()
        {
            TotalItemQuantityCount = LineItemsCollection.Sum(item => item.PurchaseQuantity);
        }

        /// <summary>
        /// Apply discount percentage and calculate final total
        /// Updates DiscountPercentageRate, DiscountAmountApplied, and FinalTotalAmount
        /// </summary>
        /// <param name="discountPercentage">Discount rate (e.g., 10 for 10%)</param>
        public void ApplyDiscountAndCalculateTotal(decimal discountPercentage)
        {
            DiscountPercentageRate = discountPercentage;
            DiscountAmountApplied = SubtotalBeforeDiscount * (discountPercentage / 100);
            FinalTotalAmount = SubtotalBeforeDiscount - DiscountAmountApplied;
        }

        #endregion

        #region Display Properties for UI Binding

        /// <summary>
        /// Formatted transaction summary for reports
        /// Shows: "TXN-20251119-0001 | 19/11/2025 01:45 | $68.40"
        /// </summary>
        public string FormattedTransactionSummary =>
            $"{TransactionIdentifier} | {TransactionTimestamp:dd/MM/yyyy HH:mm} | ${FinalTotalAmount:F2}";

        /// <summary>
        /// Formatted timestamp for display
        /// Shows: "19/11/2025 01:45:30"
        /// </summary>
        public string FormattedTimestamp => TransactionTimestamp.ToString("dd/MM/yyyy HH:mm:ss");

        #endregion

        #region Methods

        /// <summary>
        /// Returns string representation for debugging
        /// </summary>
        public override string ToString()
        {
            return $"{TransactionIdentifier}: {TotalItemQuantityCount} items, ${FinalTotalAmount:F2}";
        }

        #endregion
    }
}
