using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class WoodsTestSuite
    {
        /*private WoodsTile _woodsTile;
        private int _initialWoodAmount;

        [SetUp]
        public void Setup()
        {
            _woodsTile = new WoodsTile();
            _initialWoodAmount = _woodsTile.WoodAmount;
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator Cut50Woods()
        {
            _woodsTile.CutWood(50);
            Assert.AreEqual(_initialWoodAmount - 50, _woodsTile.WoodAmount);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CutUntilEmptyButDontGoNegative()
        {
            while(_woodsTile.WoodAmount > 0)
            {
                _woodsTile.CutWood(50);
            }
            // Cut one more wood
            _woodsTile.CutWood(1);
            Assert.AreEqual(0, _woodsTile.WoodAmount);
            yield return null;
        }*/
    }
}
