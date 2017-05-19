//==================================================
// 
//  Copyright 2012 Siemens Product Lifecycle Management Software Inc. All Rights Reserved.
//
//==================================================




using System;
using System.Collections;

using Teamcenter.ClientX;
using Teamcenter.Schemas.Soa._2006_03.Exceptions;

// Include the Data Management Service Interface
using Teamcenter.Services.Strong.Core;

// Input and output structures for the service operations
// Note: the different namespace from the service interface
using Teamcenter.Services.Strong.Core._2006_03.DataManagement;
using Teamcenter.Services.Strong.Core._2007_01.DataManagement;
using Teamcenter.Services.Strong.Core._2008_06.DataManagement;

using Teamcenter.Soa.Client.Model;
using Teamcenter.Soa.Exceptions;

using Item         = Teamcenter.Soa.Client.Model.Strong.Item;
using ItemRevision = Teamcenter.Soa.Client.Model.Strong.ItemRevision;
using HelloTeamcenter.form;

namespace Teamcenter.Hello
{

/**
 * Perform different operations in the DataManamentService
 *
 */
public class DataManagement
{
    private ExampleForm exampleForm;

    /**
     * Perform a sequence of data management operations: Create Items, Revise
     * the Items, and Delete the Items
     *
     */
    public void createReviseAndDelete()
    {
        try
        {
            int numberOfItems = 3;

            // Reserve Item IDs and Create Items with those IDs
            ItemIdsAndInitialRevisionIds[] itemIds = generateItemIds(numberOfItems, "Item");
            CreateItemsOutput[] newItems = createItems(itemIds, "Item");

            // Copy the Item and ItemRevision to separate arrays for further
            // processing
            Item[] items = new Item[newItems.Length];
            ItemRevision[] itemRevs = new ItemRevision[newItems.Length];
           
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = newItems[i].Item;
                itemRevs[i] = newItems[i].ItemRev;
            }

            // Reserve revision IDs and revise the Items
            Hashtable allRevIds = generateRevisionIds(items);
            reviseItems(allRevIds, itemRevs);

            // Delete all objects created
            deleteItems(items);
        }
        catch (ServiceException e)
        {
            exampleForm.appendTxt(e.Message );
        }

    }

    /**
     * Reserve a number Item and Revision Ids
     *
     * @param numberOfIds      Number of IDs to generate
     * @param type             Type of IDs to generate
     *
     * @return An array of Item and Revision IDs. The size of the array is equal
     *         to the input numberOfIds
     *
     * @throws ServiceException   If any partial errors are returned
     */
    public ItemIdsAndInitialRevisionIds[] generateItemIds(int numberOfIds, String type)
    //        throws ServiceException
    {
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());

        GenerateItemIdsAndInitialRevisionIdsProperties[] properties = new GenerateItemIdsAndInitialRevisionIdsProperties[1];
        GenerateItemIdsAndInitialRevisionIdsProperties property = new GenerateItemIdsAndInitialRevisionIdsProperties();

        property.Count = numberOfIds;
        property.ItemType = type;
        property.Item = null; // Not used
        properties[0] = property;

        // *****************************
        // Execute the service operation
        // *****************************
        GenerateItemIdsAndInitialRevisionIdsResponse response = dmService.GenerateItemIdsAndInitialRevisionIds(properties);



        // The AppXPartialErrorListener is logging the partial errors returned
        // In this simple example if any partial errors occur we will throw a
        // ServiceException
        if (response.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException( "DataManagementService.generateItemIdsAndInitialRevisionIds returned a partial error.");

        // The return is a map of ItemIdsAndInitialRevisionIds keyed on the
        // 0-based index of requested IDs. Since we only asked for IDs for one
        // data type, the map key is '0'
        Int32 bIkey = 0;
        Hashtable allNewIds = response.OutputItemIdsAndInitialRevisionIds;
        ItemIdsAndInitialRevisionIds[] myNewIds = (ItemIdsAndInitialRevisionIds[]) allNewIds[bIkey];

        return myNewIds;
    }

    /**
     * Create Items
     *
     * @param itemIds        Array of Item and Revision IDs
     * @param itemType       Type of item to create
     *
     * @return Set of Items and ItemRevisions
     *
     * @throws ServiceException  If any partial errors are returned
     */
    public CreateItemsOutput[] createItems(ItemIdsAndInitialRevisionIds[] itemIds, String itemType)
     //       throws ServiceException
    {
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());
        // Populate form type
        GetItemCreationRelatedInfoResponse relatedResponse = dmService.GetItemCreationRelatedInfo(itemType, null);
        String[] formTypes = new String[0];
        if ( relatedResponse.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException( "DataManagementService.getItemCretionRelatedInfo returned a partial error.");

        formTypes = new String[relatedResponse.FormAttrs.Length];
        for ( int i = 0; i < relatedResponse.FormAttrs.Length; i++ )
        {
            FormAttributesInfo attrInfo = relatedResponse.FormAttrs[i];
            formTypes[i] = attrInfo.FormType;
        }

        ItemProperties[] itemProps = new ItemProperties[itemIds.Length];
        for (int i = 0; i < itemIds.Length; i++)
        {
            // Create form in cache for form property population
            ModelObject[] forms = createForms(itemIds[i].NewItemId, formTypes[0],
                                              itemIds[i].NewRevId, formTypes[1],
                                              null, false);
            ItemProperties itemProperty = new ItemProperties();

            itemProperty.ClientId = "AppX-Test";
            itemProperty.ItemId = itemIds[i].NewItemId;
            itemProperty.RevId = itemIds[i].NewRevId;
            itemProperty.Name = "AppX-Test";
            itemProperty.Type = itemType;
            itemProperty.Description = "Test Item for the SOA AppX sample application.";
            itemProperty.Uom = "";

            // Retrieve one of form attribute value from Item master form.
            ServiceData serviceData = dmService.GetProperties(forms, new String[]{"project_id"});
            if ( serviceData.sizeOfPartialErrors() > 0)
                throw new ServiceException( "DataManagementService.getProperties returned a partial error.");
            Property property = null;
            try
            {
                property= forms[0].GetProperty("project_id");
            }
            catch ( NotLoadedException /*ex*/){}


            // Only if value is null, we set new value
            if ( property == null || property.StringValue == null || property.StringValue.Length == 0)
            {
                itemProperty.ExtendedAttributes = new ExtendedAttributes[1];
                ExtendedAttributes theExtendedAttr = new ExtendedAttributes();
                theExtendedAttr.Attributes = new Hashtable();
                theExtendedAttr.ObjectType = formTypes[0];
                theExtendedAttr.Attributes["project_id"] = "project_id";
                itemProperty.ExtendedAttributes[0] = theExtendedAttr;
            }
            itemProps[i] = itemProperty;
        }


        // *****************************
        // Execute the service operation
        // *****************************
        CreateItemsResponse response = dmService.CreateItems(itemProps, null, "");
        // before control is returned the ChangedHandler will be called with
        // newly created Item and ItemRevisions



        // The AppXPartialErrorListener is logging the partial errors returned
        // In this simple example if any partial errors occur we will throw a
        // ServiceException
        if (response.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException( "DataManagementService.createItems returned a partial error.");

        return response.Output;
    }

    /**
     * Reserve Revision IDs
     *
     * @param items       Array of Items to reserve Ids for
     *
     * @return Map of RevisionIds
     *
     * @throws ServiceException  If any partial errors are returned
     */
    public Hashtable generateRevisionIds(Item[] items) //throws ServiceException
    {
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());

        GenerateRevisionIdsResponse response = null;
        GenerateRevisionIdsProperties[] input = null;
        input = new GenerateRevisionIdsProperties[items.Length];
      
        for (int i = 0; i < items.Length; i++)
        {
            GenerateRevisionIdsProperties property = new GenerateRevisionIdsProperties();
            property.Item = items[i];
            property.ItemType = "";
            input[i] = property;
        }

        // *****************************
        // Execute the service operation
        // *****************************
        response = dmService.GenerateRevisionIds(input);

        // The AppXPartialErrorListener is logging the partial errors returned
        // In this simple example if any partial errors occur we will throw a
        // ServiceException
        if (response.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException( "DataManagementService.generateRevisionIds returned a partial error.");

        return response.OutputRevisionIds;
    }

    /**
     * Revise Items
     *
     * @param revisionIds     Map of Revsion IDs
     * @param itemRevs        Array of ItemRevisons
     *
     * @return Map of Old ItemRevsion(key) to new ItemRevision(value)
     *
     * @throws ServiceException         If any partial errors are returned
     */
    public void reviseItems(Hashtable revisionIds, ItemRevision[] itemRevs) //throws ServiceException
    {
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());

        ReviseInfo[] reviseInfo = new ReviseInfo[itemRevs.Length];
        for (int i = 0; i < itemRevs.Length; i++)
        {
            RevisionIds rev = (RevisionIds) revisionIds[i];

            reviseInfo[i] = new ReviseInfo();
            reviseInfo[i].BaseItemRevision = itemRevs[i];
            reviseInfo[i].ClientId         = itemRevs[i].Uid+ "--" + i;
            reviseInfo[i].Description      = "describe testRevise";
            reviseInfo[i].Name             = "testRevise";
            reviseInfo[i].NewRevId         = rev.NewRevId;
        }

        // *****************************
        // Execute the service operation
        // *****************************
        ReviseResponse2 revised = dmService.Revise2(reviseInfo);
        // before control is returned the ChangedHandler will be called with
        // newly created Item and ItemRevisions



        // The AppXPartialErrorListener is logging the partial errors returned
        // In this simple example if any partial errors occur we will throw a
        // ServiceException
        if (revised.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException("DataManagementService.revise returned a partial error.");
    }

    /**
     * Delete the Items
     *
     * @param items     Array of Items to delete
     *
     * @throws ServiceException    If any partial errors are returned
     */
    public void deleteItems(Item[] items) //throws ServiceException
    {
        // Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());

        // *****************************
        // Execute the service operation
        // *****************************
        ServiceData serviceData = dmService.DeleteObjects(items);

        // The AppXPartialErrorListener is logging the partial errors returned
        // In this simple example if any partial errors occur we will throw a
        // ServiceException
        if (serviceData.sizeOfPartialErrors() > 0)
            throw new ServiceException("DataManagementService.deleteObjects returned a partial error.");
    }

    /**
     * Create ItemMasterForm and ItemRevisionMasterForm
     *
     * @param IMFormName      Name of ItemMasterForm
     * @param IMFormType      Type of ItemMasterForm
     * @param IRMFormName     Name of ItemRevisionMasterForm
     * @param IRMFormType     Type of ItemRevisionMasterForm
     * @param parent          The container object that two
     *                        newly-created forms will be added into.
     * @return ModelObject[]  Array of forms
     *
     * @throws ServiceException         If any partial errors are returned
     */
    public ModelObject[] createForms ( String IMFormName, String IMFormType,
                                String IRMFormName, String IRMFormType,
                                ModelObject parent, bool saveDB ) //throws ServiceException
    {
        //Get the service stub
        DataManagementService dmService = DataManagementService.getService(MyFormAppSession.getConnection());
        FormInfo[] inputs = new FormInfo[2];
        inputs[0] = new FormInfo();
        inputs[0].ClientId = "1";
        inputs[0].Description="";
        inputs[0].Name = IMFormName;
        inputs[0].FormType=IMFormType;
        inputs[0].SaveDB = saveDB;
        inputs[0].ParentObject = parent ;
        inputs[1] = new FormInfo();
        inputs[1].ClientId = "2";
        inputs[1].Description="";
        inputs[1].Name = IRMFormName;
        inputs[1].FormType=IRMFormType;
        inputs[1].SaveDB = saveDB;
        inputs[1].ParentObject = parent;
        CreateOrUpdateFormsResponse response = dmService.CreateOrUpdateForms(inputs);
       
        if ( response.ServiceData.sizeOfPartialErrors() > 0)
            throw new ServiceException("DataManagementService.createForms returned a partial error.");
       
        ModelObject[] forms = new ModelObject [inputs.Length];
       
        for (int i=0; i<inputs.Length; ++i)
        {
            forms[i] = response.Outputs[i].Form;
        }

        return forms;
    }

    public DataManagement(ExampleForm exampleForm)
    {
        this.exampleForm = exampleForm;
    }
}
}