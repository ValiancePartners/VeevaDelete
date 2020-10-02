//-----------------------------------------------------------------------
// <copyright file="TextBoxTraceListener.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains TextBoxTraceListener class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Trace listener for a textbox
    /// </summary>
    public class TextBoxTraceListener : TraceListener
    {
        /// <summary>
        /// the text box to record trace messages in
        /// </summary>
        /// <param name="text"></param>
        private readonly TextBox output;

        // This delegate enables asynchronous calls for setting the text property on a TextBox control.
        // delegate void SetTextCallback(string text);

        /// <summary>
        /// the delegate that sends text to the textbox
        /// </summary>
        private readonly SendOutput sendOut;

        //// <summary>
        //// placeholder ?
        //// </summary>
        ////private TextBoxBase myOwner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxTraceListener" /> class
        /// </summary>
        /// <param name="output">The textbox that should record trace messages</param>
        public TextBoxTraceListener(TextBox output)
        {
            this.output = output;
            this.sendOut = new SendOutput(this.SetText);
        }

        /// <summary>
        /// delegate type to send text
        /// </summary>
        /// <param name="text">the text to send</param>
        private delegate void SendOutput(string text);

        //// <summary>
        //// Sets private myOwner. ??? allows visual update of control???.
        //// </summary>
        ////public TextBoxBase Owner
        ////{
        ////    set { this.myOwner = value; }
        ////}

        /// <summary>
        /// Write a trace message to the listing text box
        /// </summary>
        /// <param name="message">the message to write to the textbox</param>
        public override void Write(string message)
        {
            this.output.Invoke(this.sendOut, new object[] { message });
        }

        /// <summary>
        /// Write a trace message and a new line to the listing text box
        /// </summary>
        /// <param name="message">the message to write to the textbox</param>
        public override void WriteLine(string message)
        {
            this.output.Invoke(this.sendOut, new object[] { message + Environment.NewLine });
        }

        /// <summary>
        /// Add a message to the text box listener and highlight it
        /// </summary>
        /// <param name="message">the message to add</param>
        private void SetText(string message)
        {
            // Add the message.
            this.output.Text += message;

            // Scroll to the end.
            this.output.SelectionStart = this.output.Text.Length;
            this.output.ScrollToCaret();
            this.output.Refresh();
        }
    }
}
