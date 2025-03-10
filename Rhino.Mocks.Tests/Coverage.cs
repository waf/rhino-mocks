﻿#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.Tests.Expectations;
using Rhino.Mocks.Utilities;

namespace Rhino.Mocks.Tests
{
	/// <summary>
	/// Tests needed to get full coverage
	/// They won't test meaningful stuff neccesarily
	/// </summary>
	
	public class Coverage
	{
			
		[Fact]
		public void CanSerializeObjectNotMockObjectFromThisRepostiory()
		{
			ObjectNotMockFromThisRepositoryException e = new ObjectNotMockFromThisRepositoryException("ff");
			ObjectNotMockFromThisRepositoryException other = (ObjectNotMockFromThisRepositoryException)SerializeAndDeserialize(e);
			Assert.Equal("ff", other.Message );
		}

		[Fact]
		public void CanTestConstaintForTruth()
		{
			if (Is.Null())
				return;
		}

		[Fact]
		public void TryingToGetEventRaiserFromNonEvenTrhows()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			demo.EnumNoArgs();
			AssertHelper.Throws<InvalidOperationException>(
				"The last method call EnumNoArgs was not an event add / remove method",
				() => LastCall.GetEventRaiser());
		}

		[Fact]
		public void UsingCallOriginalWithSetter()
		{
			MockRepository mocks = new MockRepository();
			WithParameters withParameters = mocks.StrictMock<WithParameters>(1);
			withParameters.Int = 5;
			LastCall.PropertyBehavior();
			mocks.ReplayAll();
			withParameters.Int = 12;
			withParameters.Int = 15;
			Assert.Equal(15, withParameters.Int);
			mocks.VerifyAll();
		}

		[Fact]
		public void MockObjCanHandleGetEventSubscribersCallsWithoutEventsRegistered()
		{
			MockRepository mocks = new MockRepository();
			WithParameters withParameters = mocks.StrictMock<WithParameters>(1);
			IMockedObject mocked = (IMockedObject) withParameters;
			Delegate eventSubscribers = mocked.GetEventSubscribers("ff");
			Assert.Null(eventSubscribers);
		}

		[Fact]
		public void MockObjEqualityCanHandleNonMockObjects()
		{
			int i = MockedObjectsEquality.Instance.GetHashCode("");
			Assert.NotEqual(0, i);
		}

		[Fact]
		public void WillDisplayMethodUsingArraysCorrectly()
		{
			string result = MethodCallUtil.StringPresentation(null, GetType().GetMethod("MethodUsingArray"), new object[] { 1, new string[] { "a", "b" } });
			Assert.Equal("Coverage.MethodUsingArray(1, [\"a\", \"b\"]);",result );
		}

		public void MethodUsingArray(int i, string[] foo)
		{
		}

		private delegate string ToStringDelegate();

		[Fact]
		public void TryingToPassNullToReturnOrThrowWithActionWillThrow()
		{
			AnyArgsExpectation expectation = new AnyArgsExpectation(new FakeInvocation(typeof(object).GetMethod("ToString")), new Range(1, 1));
			expectation.ActionToExecute = (ToStringDelegate)delegate { return "fpp"; };
			AssertHelper.Throws<InvalidOperationException>(
				"Trying to run a Do() delegate when no arguments were matched to the expectation.",
				() => expectation.ReturnOrThrow(null, null));
		}

		[Fact]
		public void CanCallSetExceptionToThrowOnVerifyOnVerifiedMockState()
		{
			new VerifiedMockState(null).SetExceptionToThrowOnVerify(null);
		}

		[Fact]
		public void WillGetPartialRecordFromPartialRecord()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			IMockState mockState = new RecordPartialMockState((IMockedObject)demo, mocks).BackToRecord();
			Assert.Equal(typeof(RecordPartialMockState), mockState.GetType());
		}

		[Fact]
		public void WillGetPartialRecordFromPartialReplay()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			IMockState mockState = new ReplayPartialMockState(new RecordPartialMockState((IMockedObject)demo, mocks)).BackToRecord();
			Assert.Equal(typeof(RecordPartialMockState), mockState.GetType());
		}

		[Fact]
		public void WillGetDynamicRecordFromDynamicReplay()
		{
			MockRepository mocks = new MockRepository();
			IDemo demo = mocks.StrictMock<IDemo>();
			IMockState mockState = new ReplayDynamicMockState(new RecordDynamicMockState((IMockedObject)demo, mocks)).BackToRecord();
			Assert.Equal(typeof(RecordDynamicMockState), mockState.GetType());
		}


		public static object SerializeAndDeserialize(object proxy)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, proxy);
			stream.Position = 0;
			return formatter.Deserialize(stream);
		}

	}
}
