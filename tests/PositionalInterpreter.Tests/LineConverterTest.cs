using PositionalInterpreter.Core;
using System;
using Xunit;

namespace PositionalInterpreter.Tests
{
    public class LineConverterTest
    {
        [Fact]
        public void DeserializeToObject()
        {
            //arrange
            string name = "FAKE NAME LTDA      ";
            string document = "0021587459";
            string patrimony = "00789953212";
            string created = "19950519";
            string line = $"{name}{document}{created}{patrimony}";

            //act
            var fakeObj = LineConverter.Deserialize<FakeForDeserialize>(line);

            //assert
            Assert.NotNull(fakeObj);
            Assert.Equal(name.Trim(), fakeObj.Name);
            Assert.Equal(document, fakeObj.Document);
            Assert.Equal(created, fakeObj.Created.ToString("yyyyMMdd"));
            Assert.Equal(7_899_532.12m, fakeObj.Patrimony);
        }

        [Fact]
        public void SerializeToString()
        {
            //arrange
            var fakeObj = new FakeForDeserialize
            {
                Name = "FAKE NAME LTDA",
                Document = "0021587459",
                Created = new DateTime(1995, 5, 19),
                Patrimony = 7_899_532.12m
            };

            string name = "FAKE NAME LTDA      ";
            string document = "0021587459";
            string patrimony = "07899532,12";
            string created = "19950519";
            string expectedLine = $"{name}{document}{created}{patrimony}";

            //act
            var fakeObjLine = LineConverter.Serialize(fakeObj);

            //assert
            Assert.NotEmpty(fakeObjLine);
            Assert.Equal(expectedLine, fakeObjLine);
        }

        #region TEMPLATE OBJECT
        [Line(LineType.Item, 50)]
        private class FakeForDeserialize
        {
            [Row(1, 20)]
            public string Name { get; set; }

            [Row(21, 10)]
            public string Document { get; set; }

            [Row(31, 8, "yyyyMMdd")]
            public DateTime Created { get; set; }

            [Row(39, 11, 2, "pt-br")]
            public decimal Patrimony { get; set; }
        }
        #endregion
    }
}
