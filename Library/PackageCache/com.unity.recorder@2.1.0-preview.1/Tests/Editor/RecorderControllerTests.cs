using System;
using UnityEngine;
using NUnit.Framework;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder.Tests
{
	class RecorderControllerTests
	{
		[Test]
		public void StartRecording_InNonPlayMode_ShouldThrowsException()
		{
			var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
			var recorderController = new RecorderController(settings);

			var ex = Assert.Throws<Exception>(() => recorderController.StartRecording());
			Assert.IsTrue(ex.Message.Contains("Start Recording can only be called in Playmode"));
			Assert.IsFalse(recorderController.IsRecording());
			
			UnityObject.DestroyImmediate(settings);
		}
	}
}
