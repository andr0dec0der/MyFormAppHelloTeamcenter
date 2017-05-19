using System;

using Teamcenter.Soa.Client.Model;
using System.Windows.Forms;


namespace Teamcenter.ClientX
{
    public class MyFormAppPartialErrorListener : PartialErrorListener
    {
        public void HandlePartialError(ErrorStack[] stacks)
        {
            if (stacks.Length == 0) return;

            MessageBox.Show("Partial Errors caught in com.teamcenter.clientx.AppXPartialErrorListener.");

            for (int i = 0; i < stacks.Length; i++)
            {
                ErrorValue[] errors = stacks[i].ErrorValues;
                MessageBox.Show("Partial Error for ");

                if (stacks[i].HasAssociatedObject() )
                {
                    MessageBox.Show("object " + stacks[i].AssociatedObject.Uid);
                }
                else if (stacks[i].HasClientId())
                {
                    MessageBox.Show("client id " + stacks[i].ClientId);
                }
                else if (stacks[i].HasClientIndex())
                {
                    MessageBox.Show("client index " + stacks[i].ClientIndex);
                }

                for (int j = 0; j < errors.Length; j++)
                {
                    MessageBox.Show("    Code: " + errors[j].Code + "\tSeverity: " + errors[j].Level + "\t" + errors[j].Message);
                }
            }
        }
    }
}
