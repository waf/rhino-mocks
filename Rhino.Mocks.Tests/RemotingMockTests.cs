#region license

// Copyright (c) 2007 Ivan Krivyakov (ivan@ikriv.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
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
using Xunit;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Tests
{
    
    public class StrictMockTests
    {
        public class TestClass : MarshalByRefObject
        {
            public TestClass(string unused)
            {
                throw new InvalidCastException("Real method should never be called"); 
            }

            public void Method() 
            { 
                throw new InvalidCastException("Real method should never be called"); 
            }

            public int MethodReturningInt()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodReturningString()
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string MethodGettingParameters(int intParam, string stringParam)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public void MethodAcceptingTestClass(TestClass other)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public int GenericMethod<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodReturningGenericType<T>(string parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public T GenericMethodWithGenericParam<T>( T parameter )
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public string StringProperty
            {
                get
                {
                    throw new InvalidCastException("Real method should never be called");
                }
                set
                {
                    throw new InvalidCastException("Real method should never be called");
                }
            }
        }

        public class GenericTestClass<T> : MarshalByRefObject
        {
            public int Method(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }

            public U GenericMethod<U>(T parameter)
            {
                throw new InvalidCastException("Real method should never be called");
            }
        }

        [Fact]
        public void CanMockVoidMethod()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            t.Method();
            mocks.ReplayAll();
            t.Method();
            mocks.VerifyAll();
        }

        [Fact]
        public void ThrowOnUnexpectedVoidMethod()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            mocks.ReplayAll();
            
        	AssertHelper.Throws<ExpectationViolationException>(
        		"TestClass.Method(); Expected #0, Actual #1.",
				() => t.Method());
        }

        [Fact]
        public void CanMockMethodReturningInt()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.MethodReturningInt()).Return(42);
            mocks.ReplayAll();
            Assert.Equal(42, t.MethodReturningInt());
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockMethodReturningString()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.MethodReturningString()).Return("foo");
            mocks.ReplayAll();
            Assert.Equal("foo", t.MethodReturningString());
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockMethodGettingParameters()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.MethodGettingParameters(42, "foo")).Return("bar");
            mocks.ReplayAll();
            Assert.Equal("bar", t.MethodGettingParameters(42, "foo"));
            mocks.VerifyAll();
        }

        [Fact]
        public void CanRejectIncorrectParameters()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.MethodGettingParameters(42, "foo")).Return("bar");
            mocks.ReplayAll();
            AssertHelper.Throws<ExpectationViolationException>(
        		"TestClass.MethodGettingParameters(19, \"foo\"); Expected #0, Actual #1.\r\nTestClass.MethodGettingParameters(42, \"foo\"); Expected #1, Actual #0.",
				() => t.MethodGettingParameters(19, "foo"));
        }

        [Fact]
        public void CanMockPropertyGet()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.StringProperty).Return("foo");
            mocks.ReplayAll();
            Assert.Equal("foo", t.StringProperty);
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockPropertySet()
        {
            MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            t.StringProperty = "foo";
            mocks.ReplayAll();
            t.StringProperty = "foo";
            mocks.VerifyAll();
        }

        [Fact]
        public void CanRejectIncorrectPropertySet()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            t.StringProperty = "foo";
            mocks.ReplayAll();
            AssertHelper.Throws<ExpectationViolationException>(
        		"TestClass.set_StringProperty(\"bar\"); Expected #0, Actual #1.\r\nTestClass.set_StringProperty(\"foo\"); Expected #1, Actual #0.",
				() => t.StringProperty = "bar");
        }

        [Fact]
        public void CanMockGenericClass()
        {
            MockRepository mocks = new MockRepository();
            GenericTestClass<string> t = (GenericTestClass<string>)mocks.StrictMock(typeof(GenericTestClass<string>));
            Expect.Call(t.Method("foo")).Return(42);
            mocks.ReplayAll();
            Assert.Equal(42, t.Method("foo"));
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockGenericMethod()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.GenericMethod<string>("foo")).Return(42);
            mocks.ReplayAll();
            Assert.Equal(42, t.GenericMethod<string>("foo"));
            mocks.VerifyAll();
        }

		[Fact]
		public void CanMockGenericMethod_WillErrorOnWrongType()
		{
			MockRepository mocks = new MockRepository();
			TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
			Expect.Call(t.GenericMethod<string>("foo")).Return(42);
			mocks.ReplayAll();
			
			AssertHelper.Throws<ExpectationViolationException>(
				@"TestClass.GenericMethod<System.Int32>(""foo""); Expected #1, Actual #1.
TestClass.GenericMethod<System.String>(""foo""); Expected #1, Actual #0.",
				() => Assert.Equal(42, t.GenericMethod<int>("foo")));
		}

        [Fact]
        public void CanMockGenericMethodReturningGenericType()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.GenericMethodReturningGenericType<string>("foo")).Return("bar");
            mocks.ReplayAll();
            Assert.Equal("bar", t.GenericMethodReturningGenericType<string>("foo"));
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockGenericMethodWithGenericParam()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = (TestClass)mocks.StrictMock(typeof(TestClass));
            Expect.Call(t.GenericMethodWithGenericParam<string>("foo")).Return("bar");
            mocks.ReplayAll();
            Assert.Equal("bar", t.GenericMethodWithGenericParam("foo"));
            mocks.VerifyAll();
        }

        [Fact]
        public void CanMockGenericMethodInGenericClass()
        {
            MockRepository mocks = new MockRepository();
            GenericTestClass<string> t = mocks.StrictMock<GenericTestClass<string>>();
            Expect.Call(t.GenericMethod<int>("foo")).Return(42);
            mocks.ReplayAll();
            Assert.Equal(42, t.GenericMethod<int>("foo"));
            mocks.VerifyAll();
        }

		[Fact]
		public void CanMockAppDomain()
		{
			MockRepository mocks = new MockRepository();
			AppDomain appDomain = mocks.StrictMock<AppDomain>();
			Expect.Call(appDomain.BaseDirectory).Return("/home/user/ayende");
			mocks.ReplayAll();
			Assert.Equal(appDomain.BaseDirectory, "/home/user/ayende" );
			mocks.VerifyAll();
		}

		[Fact]
    	public void NotCallingExpectedMethodWillCauseVerificationError()
    	{
			MockRepository mocks = new MockRepository();
			AppDomain appDomain = mocks.StrictMock<AppDomain>();
			Expect.Call(appDomain.BaseDirectory).Return("/home/user/ayende");
			mocks.ReplayAll();
			AssertHelper.Throws<ExpectationViolationException>(@"AppDomain.get_BaseDirectory(); Expected #1, Actual #0.",
			                                             () => mocks.VerifyAll());
    	}

        [Fact]
        public void CanMockMethodAcceptingTestClass()
        {
            MockRepository mocks = new MockRepository();
            TestClass t1 = mocks.StrictMock<TestClass>();
            TestClass t2 = mocks.StrictMock<TestClass>();
            t1.MethodAcceptingTestClass(t2);
            mocks.ReplayAll();
            t1.MethodAcceptingTestClass(t2);
            mocks.VerifyAll();
        }

        [Fact]
        // can't use ExpectedException since expected message is dynamic
        public void CanMockMethodAcceptingTestClass_WillErrorOnWrongParameter()
        {
            string t2Text = "@";
            string t3Text = "@";

            try
            {

                MockRepository mocks = new MockRepository();
                TestClass t1 = mocks.StrictMock<TestClass>();
                TestClass t2 = mocks.StrictMock<TestClass>();
                TestClass t3 = mocks.StrictMock<TestClass>();
                t2Text = t2.ToString();
                t3Text = t3.ToString();

                t1.MethodAcceptingTestClass(t2);
                mocks.ReplayAll();
                t1.MethodAcceptingTestClass(t3);
                mocks.VerifyAll();

                Assert.False(true, "Expected ExpectationViolationException");
            }
            catch (ExpectationViolationException ex)
            {
                string msg =
                    String.Format("TestClass.MethodAcceptingTestClass({0}); Expected #0, Actual #1.\r\n" +
                                  "TestClass.MethodAcceptingTestClass({1}); Expected #1, Actual #0.",
                                  t3Text,
                                  t2Text);

                Assert.Equal(msg, ex.Message);
            }
        }

        [Fact]
        public void StrictMockGetTypeReturnsMockedType()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = mocks.StrictMock<TestClass>();
            Assert.Same(typeof(TestClass), t.GetType());
        }

        [Fact]
        public void StrictMockGetHashCodeWorks()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = mocks.StrictMock<TestClass>();
            t.GetHashCode();
        }

        [Fact]
        public void StrictMockToStringReturnsDescription()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = mocks.StrictMock<TestClass>();
            int hashCode = t.GetHashCode();
            string toString = t.ToString();
            Assert.Equal(String.Format("RemotingMock_{0}<TestClass>", hashCode), toString);
        }

        [Fact]
        public void StrictMockEquality()
        {
            MockRepository mocks = new MockRepository();
            TestClass t = mocks.StrictMock<TestClass>();

            Assert.False(t.Equals(null));
            Assert.False(t.Equals(42));
            Assert.False(t.Equals("foo"));
            Assert.True(t.Equals(t));
        }
    }
}
