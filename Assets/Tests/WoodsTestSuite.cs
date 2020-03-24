using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class WoodsTestSuite
    {
        private WoodsTile[] _woodsTile;
        private SawmillTile _sawmillTile;
        private WoodsTile _firstTile;

        [SetUp]
        public void Setup()
        {
            _woodsTile = new WoodsTile[8];
            for(int i = 0; i < _woodsTile.Length; i++)
            {
                _woodsTile[i] = new WoodsTile();
            }
            _firstTile = _woodsTile[0];
            _sawmillTile = new SawmillTile();
        }

        private void SetWoodsValues(int[] values) 
        {
            for(int i = 0; i < _woodsTile.Length; i++)
            {
                if(i < values.Length)
                {
                    _woodsTile[i]._resourceAmount = values[i];
                }
            }
        }

        private void AssertWoodValues(int[] values)
        {
            for(int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], _woodsTile[i].WoodAmount());
            }
        }


        [UnityTest]
        public IEnumerator InitIsOK()
        {
            // Not really useful at the moment
            int initWoodAmount = _firstTile.WoodAmount();
            Assert.LessOrEqual(initWoodAmount, 500);
            Assert.GreaterOrEqual(initWoodAmount, 300);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Cut50Woods()
        {
            int initialWoodAmount = _firstTile.WoodAmount();
            _firstTile.CutResource(50);
            Assert.AreEqual(initialWoodAmount - 50, _firstTile.WoodAmount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator CutUntilEmptyButDontGoNegative()
        {
            // Cut all wood
            while(_firstTile.WoodAmount() > 0)
            {
                _firstTile.CutResource(50);
            }
            // Cut one more wood
            _firstTile
            .CutResource(1);
            Assert.AreEqual(0, _firstTile.WoodAmount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator SawmillCutsEvenlyBetweenAllWoods()
        {
            int[] values = { 100, 100, 100, 100, 100, 0, 0, 0 };
            SetWoodsValues(values);

            // Sawmill cuts 100 at total
            _sawmillTile.strategy.GenerateResource(_woodsTile,100);

            int[] expectedValues = { 80, 80, 80, 80, 80, 0, 0, 0 };
            AssertWoodValues(expectedValues);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SawmillCutsEvenlyEvenWhenNotEnoughWoodOnSomeTiles()
        {
            int[] values = { 100, 50, 20, 10, 20, 50, 100, 0 };
            SetWoodsValues(values);

            // Sawmill cuts 100 at total
            _sawmillTile.strategy.GenerateResource(_woodsTile, 100);

            // Removes the 10 from the lowest tile
            // Removes (100-10)/6 = 15 from the other tiles
            int[] expectedResults = { 85, 35, 5, 0, 5, 35, 85, 0 };
            AssertWoodValues(expectedResults);

            _sawmillTile.strategy.GenerateResource(_woodsTile, 100);

            int[] newExpectedResults = { 62, 13, 0, 0, 0, 13, 62, 0 };
            AssertWoodValues(newExpectedResults);

            yield return null;
        }

        [UnityTest] 
        public IEnumerator SawmillDoesNotProvideMoreWoodThanCut()
        {
            int[] values = { 10, 10, 10, 10, 10, 10, 10, 10 };
            SetWoodsValues(values);

            // Can't cut 100 here, only 80 should be cut at total
            int totalCutWood = _sawmillTile.strategy.GenerateResource(_woodsTile, 100);

            int[] expectedValues = { 0, 0, 0, 0, 0, 0, 0, 0 };
            AssertWoodValues(expectedValues);
            Assert.AreEqual(80, totalCutWood);
            yield return null;
        }
    }
}
