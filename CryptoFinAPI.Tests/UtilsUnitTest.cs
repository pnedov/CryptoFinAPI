namespace CryptoFinAPI.UnitTests;
public class UtilsUnitTest
    {
        /// <summary>
        /// Test method convertUnixMillisecondsToDateTime(string unixTimeStamp)
        /// </summary>
        [Fact]
        public void Add_StringMilliseconds_ReturnDateTime()
        {
            //Act
            var result = Utils.convertUnixMillisecondsToDateTime("1672696800000");

            //Assert
            Assert.Equal(new DateTime(2023,01,2).Date, result.Date);
        }

        /// <summary>
        /// Test method convertUnixSecondsToDateTime(string unixTimeStamp)
        /// </summary>
        [Fact]
        public void Add_StringSeconds_ReturnDateTime()
        {
            //Act
            var result = Utils.convertUnixSecondsToDateTime("1672696800");

            //Assert
            Assert.Equal(new DateTime(2023, 01, 2).Date, result.Date);
        }

        /// <summary>
        /// Test method convertToMilliseconds(ref string from, ref string to)
        /// </summary>
        [Fact]
        public void Add_StringFromTo_ReturnMilliseconds()
        {
            //Act
            string from = "1672696800";
            string actual_from = "1672696800000";
            string to = "1723150800";
            string actual_to = "1723150800000";
            Utils.convertToMillisecondsRange(ref from, ref to);

            //Assert
            Assert.Equal(from, actual_from);
            Assert.Equal(to, actual_to);
        }

        /// <summary>
        /// Test method convertToMilliseconds2(ref string from)
        /// </summary>
        [Fact]
        public void Add_StringFrom_ReturnMilliseconds()
        {
            //Act
            string from = "1672696800";
            string actual_from = "1672696800000";
            Utils.convertToMilliseconds2(ref from);

            //Assert
            Assert.Equal(from, actual_from);

        }

        /// <summary>
        /// Test method convertToSeconds(ref string from, ref string to)
        /// </summary>
        [Fact]
        public void Add_StringFromTo_ReturnSeconds()
        {
            //Act
            string from = "1672696800000";
            string actual_from = "1672696800";
            string to = "1672696800000";
            string actual_to = "1672696800";

            Utils.convertToSeconds(ref from, ref to);

            //Assert
            Assert.Equal(from, actual_from);
            Assert.Equal(to, actual_to);
        }
}