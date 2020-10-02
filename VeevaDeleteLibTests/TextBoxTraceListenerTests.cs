using Microsoft.VisualStudio.TestTools.UnitTesting;
using VeevaDeleteApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeevaDeleteLib.Tests
{
    [TestClass()]
    public class TextBoxTraceListenerTests
    {
        [TestMethod()]
        public void TextBoxTraceListenerTest()
        {
            Form testForm = new System.Windows.Forms.Form();
            TextBox tb = new TextBox();
            testForm.Controls.Add(tb);
            testForm.Visible = false;
            testForm.Show();
            TextBoxTraceListener tbtl = new TextBoxTraceListener(tb);
            Trace.Listeners.Add(tbtl);
            string testmessage = "TestLine";
            Trace.WriteLine(testmessage);
            Trace.Flush();
            Trace.Listeners.Remove(tbtl);
            string expected = testmessage + Environment.NewLine;
            string actual = tb.Text;
            Assert.AreEqual(expected, actual);
        }
    }
}