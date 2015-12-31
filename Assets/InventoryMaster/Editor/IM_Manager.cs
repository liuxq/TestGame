using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


public class IM_Manager : EditorWindow
{
    [MenuItem("Master System/IM - Manager")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(IM_Manager));
        headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
        skypeTexture = Resources.Load<Texture>("EditorWindowTextures/skype-icon");
        emailTexture = Resources.Load<Texture>("EditorWindowTextures/email-icon");
        folderIcon = Resources.Load<Texture>("EditorWindowTextures/folder-icon");

        Object itemDatabase = Resources.Load("ItemDatabase");
        if (itemDatabase == null)
            inventoryItemList = CreateItemDatabase.createItemDatabase();
        else
            inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        Object attributeDatabase = Resources.Load("AttributeDatabase");
        if (attributeDatabase == null)
            itemAttributeList = CreateAttributeDatabase.createItemAttributeDatabase();
        else
            itemAttributeList = (ItemAttributeList)Resources.Load("AttributeDatabase");

        Object inputManager = Resources.Load("InputManager");
        if (inputManager == null)
            inputManagerDatabase = CreateInputManager.createInputManager();
        else
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");


    }

    static Texture skypeTexture;
    static Texture emailTexture;
    static Texture headTexture;
    static Texture folderIcon;

    bool showInputManager;
    bool showItemDataBase;
    bool showBluePrintDataBase;

    //Itemdatabase
    static ItemDataBaseList inventoryItemList = null;
    static ItemAttributeList itemAttributeList = null;
    static InputManager inputManagerDatabase = null;
    List<bool> manageItem = new List<bool>();

    Vector2 scrollPosition;

    static KeyCode test;

    bool showItemAttributes;
    string addAttributeName = "";
    int attributeAmount = 1;
    int[] attributeName;
    int[] attributeValue;

    int[] attributeNamesManage = new int[100];
    int[] attributeValueManage = new int[100];
    int attributeAmountManage;

    bool showItem;

    public int toolbarInt = 0;
    public string[] toolbarStrings = new string[] { "Create Items", "Manage Items" };

    //Blueprintdatabase
    static BlueprintDatabase bluePrintDatabase = null;
    List<bool> manageItem1 = new List<bool>();
    int amountOfFinalItem;
    //    float timeToCraft;
    int finalItemID;
    int amountofingredients;
    int[] ingredients;
    int[] amount;
    ItemDataBaseList itemdatabase;
    Vector2 scrollPosition1;

    public int toolbarInt1 = 0;
    public string[] toolbarStrings1 = new string[] { "Create Blueprints", "Manage Blueprints" };

    void OnGUI()
    {
        Header();

        if (GUILayout.Button("Input Manager"))
        {
            showInputManager = !showInputManager;
            showItemDataBase = false;
            showBluePrintDataBase = false;
        }

        if (showInputManager)
            InputManager1();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Itemdatabase"))
        {
            showInputManager = false;
            showItemDataBase = !showItemDataBase;
            showBluePrintDataBase = false;
        }

        if (GUILayout.Button("Blueprintdatabase"))
        {
            showInputManager = false;
            showItemDataBase = false;
            showBluePrintDataBase = !showBluePrintDataBase;
        }
        EditorGUILayout.EndHorizontal();

        if (showItemDataBase)
            ItemDataBase();

        if (showBluePrintDataBase)
            BluePrintDataBase();

    }



    void InputManager1()
    {
        EditorGUILayout.BeginVertical("Box");
        GUILayout.Label("┌─Inputs", EditorStyles.boldLabel);

        EditorUtility.SetDirty(inputManagerDatabase);

        inputManagerDatabase.InventoryKeyCode = (KeyCode)EditorGUILayout.EnumPopup("Inventory", (KeyCode)inputManagerDatabase.InventoryKeyCode);
        inputManagerDatabase.StorageKeyCode = (KeyCode)EditorGUILayout.EnumPopup("Storage", (KeyCode)inputManagerDatabase.StorageKeyCode);
        inputManagerDatabase.CharacterSystemKeyCode = (KeyCode)EditorGUILayout.EnumPopup("Charactersystem", (KeyCode)inputManagerDatabase.CharacterSystemKeyCode);
        inputManagerDatabase.CraftSystemKeyCode = (KeyCode)EditorGUILayout.EnumPopup("Craftsystem", (KeyCode)inputManagerDatabase.CraftSystemKeyCode);
        inputManagerDatabase.SplitItem = (KeyCode)EditorGUILayout.EnumPopup("Split", (KeyCode)inputManagerDatabase.SplitItem);

        //if(inputManagerDatabase.UFPS)
        //{
        //    inputManagerDatabase.throwGrenade = (KeyCode)EditorGUILayout.EnumPopup("Grenade", (KeyCode)inputManagerDatabase.throwGrenade);
        //    inputManagerDatabase.reloadWeapon = (KeyCode)EditorGUILayout.EnumPopup("Reload", (KeyCode)inputManagerDatabase.reloadWeapon);
        //}

        EditorUtility.SetDirty(inputManagerDatabase);

        EditorGUILayout.EndVertical();
    }

    void ItemDataBase()
    {
        EditorGUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Width(position.width - 18));                                                    //creating a toolbar(tabs) to navigate what you wanna do
        GUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);

        if (toolbarInt == 0)                                                                                                                              //if equal 0 than it is "Create Item"
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Add Item", GUILayout.Width(position.width - 23)))
            {
                addItem();
                showItem = true;
            }

            if (showItem)
            {
                GUI.color = Color.white;

                GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));
                try
                {
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemName = EditorGUILayout.TextField("Item Name", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemName, GUILayout.Width(position.width - 30));          //textfield for the itemname which you wanna create
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemID = inventoryItemList.itemList.Count - 1;                                             //itemID getting set automaticly ...its unique...better do not change it :D  

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Item Description");                                                                                                                        //label ItemDescription
                    GUILayout.Space(47);
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemDesc = EditorGUILayout.TextArea(inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemDesc, GUILayout.Width(position.width - 180), GUILayout.Height(70));     //Text area for the itemDesc
                    GUILayout.EndHorizontal();
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemIcon, typeof(Sprite), false, GUILayout.Width(position.width - 33));         //objectfield for the itemicon for your new item
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemModel = (GameObject)EditorGUILayout.ObjectField("Item Model", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemModel, typeof(GameObject), false, GUILayout.Width(position.width - 33));      //objectfield for the itemmodel for your new item

                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemType, GUILayout.Width(position.width - 33));                                      //the itemtype which you want to have can be selected with the enumpopup
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].maxStack = EditorGUILayout.IntField("Max Stack", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].maxStack, GUILayout.Width(position.width - 33));
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].rarity = EditorGUILayout.IntSlider("Rarity", inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].rarity, 0, 100);
                    GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 33));
                    showItemAttributes = EditorGUILayout.Foldout(showItemAttributes, "Item attributes");
                    if (showItemAttributes)
                    {
                        GUILayout.BeginHorizontal();
                        addAttributeName = EditorGUILayout.TextField("Name", addAttributeName);
                        GUI.color = Color.green;
                        if (GUILayout.Button("Add"))
                            addAttribute();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                        GUI.color = Color.white;
                        EditorGUI.BeginChangeCheck();
                        attributeAmount = EditorGUILayout.IntSlider("Amount", attributeAmount, 0, 50);
                        if (EditorGUI.EndChangeCheck())
                        {
                            attributeName = new int[attributeAmount];
                            attributeValue = new int[attributeAmount];
                        }

                        string[] attributes = new string[itemAttributeList.itemAttributeList.Count];
                        for (int i = 1; i < attributes.Length; i++)
                        {
                            attributes[i] = itemAttributeList.itemAttributeList[i].attributeName;
                        }


                        for (int k = 0; k < attributeAmount; k++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            attributeName[k] = EditorGUILayout.Popup("Attribute " + (k + 1), attributeName[k], attributes, EditorStyles.popup);
                            attributeValue[k] = EditorGUILayout.IntField("Value", attributeValue[k]);
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Save"))
                        {
                            List<ItemAttribute> iA = new List<ItemAttribute>();
                            for (int i = 0; i < attributeAmount; i++)
                            {
                                iA.Add(new ItemAttribute(attributes[attributeName[i]], attributeValue[i]));
                            }
                            inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].itemAttributes = iA;
                            showItem = false;

                        }

                    }
                    GUILayout.EndVertical();
                    inventoryItemList.itemList[inventoryItemList.itemList.Count - 1].indexItemInList = 999;



                }
                catch { }
                GUILayout.EndVertical();
            }

        }

        if (toolbarInt == 1)                                                            //if toolbar equals 1...manage items...alle items are editable here
        {
            if (inventoryItemList == null)
                inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");
            if (inventoryItemList.itemList.Count == 1)                                  //if there is no item in the database you get this...yes it is right to have == 1
            {
                GUILayout.Label("There is no Item in the Database!");                   //information that you do not have one
            }
            else
            {
                GUILayout.BeginVertical();
                for (int i = 1; i < inventoryItemList.itemList.Count; i++)                      //get through all items which are there in the itemdatabase
                {
                    try
                    {
                        manageItem.Add(false);                                                                                                  //foldout is closed at default
                        GUILayout.BeginVertical("Box");
                        manageItem[i] = EditorGUILayout.Foldout(manageItem[i], "" + inventoryItemList.itemList[i].itemName);                   //create for every item which you have in the itemdatabase a foldout
                        if (manageItem[i])                                                                                                      //if you press on it you get this
                        {

                            EditorUtility.SetDirty(inventoryItemList);                                                                          //message the scriptableobject that you change something now                                                                                                
                            GUI.color = Color.red;                                                                                              //all upcoming GUIelements get changed to red
                            if (GUILayout.Button("Delete Item"))                                           //create button that deletes the item
                            {
                                inventoryItemList.itemList.RemoveAt(i);                                                                         //remove the item out of the itemdatabase
                                EditorUtility.SetDirty(inventoryItemList);                                                                      //and message the database again that you changed something
                            }

                            GUI.color = Color.white;                                                                                            //next GUIElements will be white
                            inventoryItemList.itemList[i].itemName = EditorGUILayout.TextField("Item Name", inventoryItemList.itemList[i].itemName, GUILayout.Width(position.width - 45));          //textfield for the itemname which you wanna create
                            inventoryItemList.itemList[i].itemID = i;                                             //itemID getting set automaticly ...its unique...better do not change it :D  
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Item ID");
                            GUILayout.Label("" + i);
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Item Description");                                                                                                                        //label ItemDescription
                            GUILayout.Space(47);
                            inventoryItemList.itemList[i].itemDesc = EditorGUILayout.TextArea(inventoryItemList.itemList[i].itemDesc, GUILayout.Width(position.width - 195), GUILayout.Height(70));     //Text area for the itemDesc
                            GUILayout.EndHorizontal();
                            inventoryItemList.itemList[i].itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", inventoryItemList.itemList[i].itemIcon, typeof(Sprite), false, GUILayout.Width(position.width - 45));         //objectfield for the itemicon for your new item
                            inventoryItemList.itemList[i].itemModel = (GameObject)EditorGUILayout.ObjectField("Item Model", inventoryItemList.itemList[i].itemModel, typeof(GameObject), false, GUILayout.Width(position.width - 45));      //objectfield for the itemmodel for your new item
                            inventoryItemList.itemList[i].itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", inventoryItemList.itemList[i].itemType, GUILayout.Width(position.width - 45));                                      //the itemtype which you want to have can be selected with the enumpopup
                            inventoryItemList.itemList[i].maxStack = EditorGUILayout.IntField("Max Stack", inventoryItemList.itemList[i].maxStack, GUILayout.Width(position.width - 45));
                            inventoryItemList.itemList[i].rarity = EditorGUILayout.IntSlider("Rarity", inventoryItemList.itemList[i].rarity, 0, 100);
                            GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 45));
                            showItemAttributes = EditorGUILayout.Foldout(showItemAttributes, "Item attributes");
                            if (showItemAttributes)
                            {

                                string[] attributes = new string[itemAttributeList.itemAttributeList.Count];
                                for (int t = 1; t < attributes.Length; t++)
                                {
                                    attributes[t] = itemAttributeList.itemAttributeList[t].attributeName;
                                }


                                if (inventoryItemList.itemList[i].itemAttributes.Count != 0)
                                {
                                    for (int t = 0; t < inventoryItemList.itemList[i].itemAttributes.Count; t++)
                                    {
                                        for (int z = 1; z < attributes.Length; z++)
                                        {
                                            if (inventoryItemList.itemList[i].itemAttributes[t].attributeName == attributes[z])
                                            {
                                                attributeNamesManage[t] = z;
                                                attributeValueManage[t] = inventoryItemList.itemList[i].itemAttributes[t].attributeValue;
                                                break;
                                            }
                                        }
                                    }
                                }

                                for (int z = 0; z < inventoryItemList.itemList[i].itemAttributes.Count; z++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUI.color = Color.red;
                                    if (GUILayout.Button("-"))
                                        inventoryItemList.itemList[i].itemAttributes.RemoveAt(z);
                                    GUI.color = Color.white;
                                    attributeNamesManage[z] = EditorGUILayout.Popup(attributeNamesManage[z], attributes, EditorStyles.popup);
                                    inventoryItemList.itemList[i].itemAttributes[z].attributeValue = EditorGUILayout.IntField("Value", inventoryItemList.itemList[i].itemAttributes[z].attributeValue);
                                    EditorGUILayout.EndHorizontal();
                                }
                                GUI.color = Color.green;
                                if (GUILayout.Button("+"))
                                    inventoryItemList.itemList[i].itemAttributes.Add(new ItemAttribute());




                                GUI.color = Color.white;
                                if (GUILayout.Button("Save"))
                                {
                                    List<ItemAttribute> iA = new List<ItemAttribute>();
                                    for (int k = 0; k < inventoryItemList.itemList[i].itemAttributes.Count; k++)
                                    {
                                        iA.Add(new ItemAttribute(attributes[attributeNamesManage[k]], attributeValueManage[k]));
                                    }
                                    inventoryItemList.itemList[i].itemAttributes = iA;

                                    GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
                                    for (int z = 0; z < items.Length; z++)
                                    {
                                        ItemOnObject item = items[z].GetComponent<ItemOnObject>();
                                        if (item.item.itemID == inventoryItemList.itemList[i].itemID)
                                        {
                                            int value = item.item.itemValue;
                                            item.item = inventoryItemList.itemList[i];
                                            item.item.itemValue = value;
                                        }
                                    }

                                    manageItem[i] = false;
                                }



                            }
                            GUILayout.EndVertical();

                            EditorUtility.SetDirty(inventoryItemList);                                                                                              //message scriptable object that you have changed something
                        }
                        GUILayout.EndVertical();
                    }
                    catch { }

                }
                GUILayout.EndVertical();

            }



        }
        if (inventoryItemList == null)
            inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

    }

    void BluePrintDataBase()
    {
        EditorGUILayout.BeginVertical("Box");
        if (inventoryItemList == null)
            inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDatabase");
        if (bluePrintDatabase == null)
            bluePrintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");

        GUILayout.BeginHorizontal();
        toolbarInt1 = GUILayout.Toolbar(toolbarInt1, toolbarStrings1, GUILayout.Width(position.width - 20));                                                    //creating a toolbar(tabs) to navigate what you wanna do
        GUILayout.EndHorizontal();
        scrollPosition1 = EditorGUILayout.BeginScrollView(scrollPosition1);
        GUILayout.Space(10);

        if (toolbarInt1 == 0)                                                                                                                              //if equal 0 than it is "Create Item"
        {
            GUI.color = Color.white;
            try
            {
                GUILayout.BeginVertical("Box");
                string[] items = new string[inventoryItemList.itemList.Count];                                                      //create a string array in length of the itemcount
                for (int i = 1; i < items.Length; i++)                                                                              //go through the item array
                {
                    items[i] = inventoryItemList.itemList[i].itemName;                                                              //and paste all names into the array
                }
                EditorGUILayout.BeginHorizontal();
                finalItemID = EditorGUILayout.Popup("Final Item", finalItemID, items, EditorStyles.popup);
                amountOfFinalItem = EditorGUILayout.IntField("Value", amountOfFinalItem);
                EditorGUILayout.EndHorizontal();
                //timeToCraft = EditorGUILayout.FloatField("Time to craft", timeToCraft);
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                amountofingredients = EditorGUILayout.IntSlider("Ingredients", amountofingredients, 1, 50, GUILayout.Width(position.width - 38));
                if (EditorGUI.EndChangeCheck())
                {
                    ingredients = new int[amountofingredients];
                    amount = new int[amountofingredients];
                }
                for (int i = 0; i < amountofingredients; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    ingredients[i] = EditorGUILayout.Popup("Ingredient " + i, ingredients[i], items, EditorStyles.popup, GUILayout.Width((position.width / 2) - 20));
                    amount[i] = EditorGUILayout.IntField("Value", amount[i], GUILayout.Width((position.width / 2) - 20));

                    EditorGUILayout.EndHorizontal();
                }
                GUI.color = Color.green;
                if (GUILayout.Button("Add Blueprint", GUILayout.Width(position.width - 35), GUILayout.Height(50)))
                    addBlueprint();

                GUILayout.EndVertical();

            }
            catch { }

        }

        if (toolbarInt1 == 1)
        {

            if (bluePrintDatabase == null)
            {
                bluePrintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");
                if (bluePrintDatabase == null)
                {
                    bluePrintDatabase = CreateBlueprintDatabase.createBlueprintDatabase();
                    bluePrintDatabase = (BlueprintDatabase)Resources.Load("BlueprintDatabase");
                }
            }

            if (bluePrintDatabase.blueprints.Count == 1)
            {
                GUILayout.Label("There is no Blueprint in the Database!");
            }
            else
            {
                GUILayout.BeginVertical();
                for (int i = 1; i < bluePrintDatabase.blueprints.Count; i++)
                {
                    try
                    {
                        manageItem1.Add(false);
                        GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));
                        manageItem1[i] = EditorGUILayout.Foldout(manageItem1[i], "" + bluePrintDatabase.blueprints[i].finalItem.itemName);                    //create for every item which you have in the itemdatabase a foldout
                        if (manageItem1[i])                                                                                                      //if you press on it you get this
                        {
                            EditorGUI.indentLevel++;
                            EditorUtility.SetDirty(bluePrintDatabase);                                                                          //message the scriptableobject that you change something now                                                                                                
                            GUI.color = Color.red;                                                                                              //all upcoming GUIelements get changed to red
                            if (GUILayout.Button("Delete Blueprint", GUILayout.Width(position.width - 38)))                                           //create button that deletes the item
                            {
                                bluePrintDatabase.blueprints.RemoveAt(i);                                                                         //remove the item out of the itemdatabase
                                EditorUtility.SetDirty(bluePrintDatabase);                                                                      //and message the database again that you changed something
                            }

                            GUI.color = Color.white;
                            EditorUtility.SetDirty(bluePrintDatabase);
                            bluePrintDatabase.blueprints[i].amountOfFinalItem = EditorGUILayout.IntField("Amount of final items", bluePrintDatabase.blueprints[i].amountOfFinalItem, GUILayout.Width(position.width - 35));
                            //bluePrintDatabase.blueprints[i].timeToCraft = EditorGUILayout.FloatField("Time to craft", bluePrintDatabase.blueprints[i].timeToCraft);
                            EditorUtility.SetDirty(bluePrintDatabase);
                            string[] items = new string[inventoryItemList.itemList.Count];                                                      //create a string array in length of the itemcount
                            for (int z = 1; z < items.Length; z++)                                                                              //go through the item array
                            {
                                items[z] = inventoryItemList.itemList[z].itemName;                                                              //and paste all names into the array
                            }
                            GUILayout.Label("Ingredients");
                            for (int k = 0; k < bluePrintDatabase.blueprints[i].ingredients.Count; k++)
                            {
                                GUILayout.BeginHorizontal();
                                GUI.color = Color.red;
                                if (GUILayout.Button("-"))
                                    bluePrintDatabase.blueprints[i].ingredients.RemoveAt(k);
                                GUI.color = Color.white;
                                bluePrintDatabase.blueprints[i].ingredients[k] = EditorGUILayout.Popup("Ingredient " + (k + 1), bluePrintDatabase.blueprints[i].ingredients[k], items, EditorStyles.popup);
                                bluePrintDatabase.blueprints[i].amount[k] = EditorGUILayout.IntField("Value", bluePrintDatabase.blueprints[i].amount[k]);

                                GUILayout.EndHorizontal();
                            }

                            GUI.color = Color.green;
                            if (GUILayout.Button("+"))
                            {
                                bluePrintDatabase.blueprints[i].ingredients.Add(0);
                                bluePrintDatabase.blueprints[i].amount.Add(0);
                            }
                            GUI.color = Color.white;
                            EditorGUI.indentLevel--;
                            EditorUtility.SetDirty(bluePrintDatabase);                                                                                              //message scriptable object that you have changed something
                        }
                        GUILayout.EndVertical();
                    }
                    catch { }
                }
                GUILayout.EndVertical();
            }

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void Header()
    {
        GUILayout.BeginHorizontal();

        if (headTexture == null || emailTexture == null || skypeTexture == null || folderIcon == null)
        {
            headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
            skypeTexture = Resources.Load<Texture>("EditorWindowTextures/skype-icon");
            emailTexture = Resources.Load<Texture>("EditorWindowTextures/email-icon");
            folderIcon = Resources.Load<Texture>("EditorWindowTextures/folder-icon");
        }

        GUI.DrawTexture(new Rect(10, 10, 75, 75), headTexture);
        GUILayout.Space(90);

        GUILayout.BeginVertical();
        GUILayout.Space(10);

        GUILayout.BeginVertical("Box");
        GUILayout.Label("┌─Informations", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUI.DrawTexture(new Rect(95, 35, 15, 15), emailTexture);
        GUILayout.Space(25);
        GUILayout.Label("buchheim.sander@yahoo.de");

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUI.DrawTexture(new Rect(95, 52, 15, 15), skypeTexture);
        GUILayout.Space(25);
        GUILayout.Label("sander.buchheim");

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUI.DrawTexture(new Rect(95, 71, 15, 15), folderIcon);
        GUILayout.Space(25);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Documentation and Script API", GUIStyle.none))
        {
            Application.OpenURL("http://spklup1991.alfahosting.org/Webversion/Scripting%20API%20Inventory%20Master.pdf");
            Application.OpenURL("http://spklup1991.alfahosting.org/Webversion/Documentation%20Inventory%20Master.pdf");
        }

        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();


        EditorGUI.BeginChangeCheck();
        if (inputManagerDatabase == null)
            inputManagerDatabase = (InputManager)Resources.Load("InputManager");

        //inputManagerDatabase.UFPS = EditorGUILayout.ToggleLeft("UFPS On/Off", inputManagerDatabase.UFPS);
        //if (EditorGUI.EndChangeCheck())
        //    setUFPSSettings();

    }

    GameObject itemPrefab;
    GameObject hotbarPrefab;

    //void setUFPSSettings()
    //{
    //    itemPrefab = Resources.Load("Prefabs/Item") as GameObject;
    //    hotbarPrefab = Resources.Load("Prefabs/Panel - Hotbar") as GameObject;

    //    if(!inputManagerDatabase.UFPS)
    //    {
    //        if (itemPrefab.GetComponent<UFPS_ConsumeItem>() != null)
    //            DestroyImmediate(itemPrefab.GetComponent<UFPS_ConsumeItem>(), true);
    //        if (hotbarPrefab.GetComponent<UFPS_Hotbar>())
    //            DestroyImmediate(hotbarPrefab.GetComponent<UFPS_Hotbar>(), true);           
    //    }
    //    else
    //    {
    //        itemPrefab.AddComponent<UFPS_ConsumeItem>();
    //        hotbarPrefab.AddComponent<UFPS_Hotbar>();            
    //    }

    //}

    void addItem()                                          //add new item to the itemdatabase
    {
        EditorUtility.SetDirty(inventoryItemList);          //message scriptable object for incoming changes
        Item newItem = new Item();                          //create a empty mask of an item
        newItem.itemName = "New Item";                      //set the name as "new Item"
        inventoryItemList.itemList.Add(newItem);            //and add this to the itemdatabase
        EditorUtility.SetDirty(inventoryItemList);          //message scriptable object that you added something
    }

    void addAttribute()
    {
        EditorUtility.SetDirty(itemAttributeList);
        ItemAttribute newAttribute = new ItemAttribute();
        newAttribute.attributeName = addAttributeName;
        itemAttributeList.itemAttributeList.Add(newAttribute);
        addAttributeName = "";
        EditorUtility.SetDirty(itemAttributeList);
    }

    void addBlueprint()                                          //add new blueprint to the database
    {
        EditorUtility.SetDirty(bluePrintDatabase);          //message scriptable object for incoming changes
        Blueprint newBlueprint = new Blueprint(ingredients.ToList<int>(), amountOfFinalItem, amount.ToList<int>(), inventoryItemList.getItemByID(finalItemID));           //create a empty mask of an item        
        bluePrintDatabase.blueprints.Add(newBlueprint);     //and add this to the itemdatabase        
        EditorUtility.SetDirty(bluePrintDatabase);          //message scriptable object that you added something
    }

}