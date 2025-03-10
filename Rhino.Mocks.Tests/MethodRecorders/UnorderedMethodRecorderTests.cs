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


using Xunit;
using Rhino.Mocks.Exceptions;
using Rhino.Mocks.Expectations;
using Rhino.Mocks.Generated;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using Rhino.Mocks.MethodRecorders;
using Rhino.Mocks.Tests.Expectations;

namespace Rhino.Mocks.Tests.MethodRecorders
{
	
	public class UnorderedMethodRecorderTests : IMethodRecorderTests
	{
		[Fact]
		public void CanRecordMethodsAndVerifyThem()
		{
			UnorderedMethodRecorder recorder = new UnorderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.Record(demo, voidNoArgs, new AnyArgsExpectation(new FakeInvocation(voidNoArgs), new Range(1, 1)));
			recorder.Record(demo, voidThreeArgs, new AnyArgsExpectation(new FakeInvocation(voidNoArgs), new Range(1, 1)));

			Assert.NotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidThreeArgs),demo, voidThreeArgs, new object[0]));
			Assert.NotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs),demo, voidNoArgs, new object[0]));
		}


		[Fact]
		public void ReplayUnrecordedMethods()
		{
			UnorderedMethodRecorder recorder = new UnorderedMethodRecorder(new ProxyMethodExpectationsDictionary());
			recorder.Record(demo, voidNoArgs, new AnyArgsExpectation(new FakeInvocation(voidNoArgs), new Range(1, 1)));
			recorder.Record(demo, voidThreeArgs, new AnyArgsExpectation(new FakeInvocation(voidNoArgs), new Range(1, 1)));

			Assert.NotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidThreeArgs),demo, voidThreeArgs, new object[0]));
			Assert.NotNull(recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs),demo, voidNoArgs, new object[0]));

			AssertHelper.Throws<ExpectationViolationException>("IDemo.VoidNoArgs(); Expected #1, Actual #2.",
			                                             () =>
			                                             recorder.GetRecordedExpectation(new FakeInvocation(voidNoArgs), demo,
			                                                                             voidNoArgs, new object[0]));
		}

		protected override IMethodRecorder CreateRecorder()
		{
			return new UnorderedMethodRecorder(new ProxyMethodExpectationsDictionary());
		}
	}
}