/// <summary>
/// This file contains the <c>PublicItemTests</c> class, which provides unit tests 
/// for the <see cref="PublicItem"/> class. The tests ensure that the <c>Quantity</c> 
/// property correctly rounds values and handles edge cases.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Models.Data_Transfer_Objects;

namespace backendtest
{
    /// <summary>
    /// The <c>PublicItemTests</c> class contains unit tests for the <see cref="PublicItem"/> model.
    /// It ensures that the <c>Quantity</c> property rounds values correctly and handles edge cases such as negatives.
    /// </summary>
    public class PublicItemTests
    {
        PublicItem publicItem;

        /// <summary>
        /// Initializes a new instance of the <c>PublicItemTests</c> class.
        /// Creates a fresh <see cref="PublicItem"/> instance before each test.
        /// </summary>
        public PublicItemTests() { 
            publicItem = new PublicItem();
        }

        /// <summary>
        /// Tests that <c>Quantity</c> rounds up correctly when the third decimal is 5 or greater.
        /// </summary>
        [Fact]
        public async Task Quantity_SetValueWithRoundingUp_RoundsToTwoDecimalPlaces()
        {
            publicItem.Quantity = 5.567f;

            float expected = 5.57f;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that <c>Quantity</c> rounds down correctly when the third decimal is less than 5.
        /// </summary>
        [Fact]
        public async Task Quantity_SetValueWithRoundingDown_RoundsToTwoDecimalPlaces()
        {            
            publicItem.Quantity = 5.564f;

            float expected = 5.56f;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that <c>Quantity</c> correctly rounds edge cases where the third decimal is exactly 5.
        /// </summary>
        [Fact]
        public async Task Quantity_SetValueWithEdgeRounding_RoundsCorrectly()
        {            
            publicItem.Quantity = 2.555f;

            float expected = 2.56f;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that setting <c>Quantity</c> to a negative value results in zero.
        /// Ensures negative values are not allowed.
        /// </summary>
        [Fact]
        public async Task Quantity_SetNegativeValue_ReturnsZero()
        {            
            publicItem.Quantity = -5.0f;

            float expected = 0;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that setting <c>Quantity</c> to zero correctly returns zero.
        /// </summary>
        [Fact]
        public async Task Quantity_SetZero_ReturnsZero()
        {            
            publicItem.Quantity = 0f;

            float expected = 0;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that setting <c>Quantity</c> to a whole number returns the same value.
        /// </summary>
        [Fact]
        public async Task Quantity_SetWholeNumber_ReturnsSameValue()
        {            
            publicItem.Quantity = 10.0f;

            float expected = 10.0f;
            Assert.Equal(expected, publicItem.Quantity);
        }

        /// <summary>
        /// Tests that setting <c>Quantity</c> to a small decimal value correctly rounds to two decimal places.
        /// </summary>
        [Fact]
        public async Task Quantity_SetSmallDecimalValue_RoundsCorrectly()
        {            
            publicItem.Quantity = 0.123f;

            float expected = 0.12f;
            Assert.Equal(expected, publicItem.Quantity);
        }
    }
}
