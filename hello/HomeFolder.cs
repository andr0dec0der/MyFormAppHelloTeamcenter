//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================

using System;

using Teamcenter.ClientX;
using Teamcenter.Services.Strong.Core;
using Teamcenter.Soa.Client.Model;
using Teamcenter.Soa.Exceptions;

using User            = Teamcenter.Soa.Client.Model.Strong.User;
using Folder          = Teamcenter.Soa.Client.Model.Strong.Folder;
using WorkspaceObject = Teamcenter.Soa.Client.Model.Strong.WorkspaceObject;
using HelloTeamcenter.form;

namespace Teamcenter.Hello
{
public class HomeFolder
{
    private ExampleForm exampleForm;

    /**
     * List the contents of the Home folder.
     * 
     */
    public void listHomeFolder(User user)
    {
        Folder home = null;
        WorkspaceObject[] contents = null;
        
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());

        try
        {
            // User was a primary object returned from the login command
            // the Object Property Policy should be configured to include the
            // 'home_folder' property. However the actuall 'home_folder' object
            // was a secondary object returned from the login request and
            // therefore does not have any properties associated with it. We will need to
            // get those properties explicitly with a 'getProperties' service request.
            home = user.Home_folder;
        }
        catch (NotLoadedException e)
        {
            exampleForm.appendTxt(e.Message);
            exampleForm.appendTxt("The Object Property Policy ($TC_DATA/soa/policies/Default.xml) is not configured with this property.");
            return;
        }

        try
        {
            ModelObject[] objects = { home };
            String[] attributes = { "contents" };
            
            // *****************************
            // Execute the service operation
            // *****************************
            dmService.GetProperties(objects, attributes);

            
            // The above getProperties call returns a ServiceData object, but it
            // just has pointers to the same object we passed into the method, so the
            // input object have been updated with new property values
            contents = home.Contents;
        }
        // This should never be thrown, since we just explicitly asked for this
        // property
        catch (NotLoadedException /*e*/){}

        exampleForm.appendTxt("");
        exampleForm.appendTxt("Home Folder:");
        exampleForm.printObjects(contents);

    }


    public HomeFolder(ExampleForm exampleForm)
    {
        // TODO: Complete member initialization
        this.exampleForm = exampleForm;
    }
}
}
