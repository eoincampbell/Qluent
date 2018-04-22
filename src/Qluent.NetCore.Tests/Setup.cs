//using System;
//using System.Collections.Generic;
//using System.Text;
//using NUnit.Framework;
//using Qluent.NetCore.Tests.Helper;

//namespace Qluent.NetCore.Tests
//{
//    [SetUpFixture]
//    public class StartStopAzureEmulator
//    {
//        private bool _wasUp;

//        [OneTimeSetUp]
//        public void StartAzureBeforeAllTestsIfNotUp()
//        {
//            if (!AzureStorageEmulatorManager.IsProcessStarted())
//            {
//                AzureStorageEmulatorManager.Start();
//                _wasUp = false;
//            }
//            else
//            {
//                _wasUp = true;
//            }

//        }

//        [OneTimeTearDown]
//        public void StopAzureAfterAllTestsIfWasDown()
//        {
//            if (!_wasUp)
//            {
//                AzureStorageEmulatorManager.Stop();
//            }
//        }
//    }
//}
