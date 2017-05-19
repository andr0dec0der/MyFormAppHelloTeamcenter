using System;
using System.Windows.Forms;
using HelloTeamcenter.form;

namespace Teamcenter.Hello {
    public class MyHello
    { 
        [STAThread]
        public static void Main(string[] args)
        {
            ExampleForm exampleForm = new ExampleForm();
            Application.Run(exampleForm);
        }
    }
}
    
    
