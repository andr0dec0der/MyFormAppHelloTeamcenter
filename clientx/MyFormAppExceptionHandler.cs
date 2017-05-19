//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================


using System;
using System.IO;

using Teamcenter.Schemas.Soa._2006_03.Exceptions;
using Teamcenter.Soa.Client;
using Teamcenter.Soa.Exceptions;
using System.Windows.Forms;

namespace Teamcenter.ClientX
{
    public class MyFormAppExceptionHandler : ExceptionHandler
    {
        public void HandleException(InternalServerException ise)
        {
            MessageBox.Show("Exception caught in com.teamcenter.clientx.AppXExceptionHandler.handleException(InternalServerException).");

            if (ise is ConnectionException)
            {
                MessageBox.Show("\nThe server returned an connection error.\n" + ise.Message
                               + "\nDo you wish to retry the last service request?[y/n]");
            }
            else if (ise is ProtocolException)
            {
                MessageBox.Show("\nThe server returned an protocol error.\n" + ise.Message
                               + "\nThis is most likely the result of a programming error."
                               + "\nDo you wish to retry the last service request?[y/n]");
            }
            else
            {
                MessageBox.Show("\nThe server returned an internal server error.\n"
                                 + ise.Message
                                 + "\nThis is most likely the result of a programming error."
                                 + "\nA RuntimeException will be thrown.");

                throw new SystemException(ise.Message);
            }
        }

            public void HandleException(CanceledOperationException coe)
        {
            MessageBox.Show("Exception caught in com.teamcenter.clientx.AppXExceptionHandler.handleException(CanceledOperationException).");

            throw new SystemException(coe.Message);
        }
    }
}
