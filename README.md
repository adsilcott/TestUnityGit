# SOCollections
Unity tools for adding scriptable objects to other scriptable objects as child objects

### Purpose
Scriptable Objects are incredibly useful for storing a variety of persistent data in Unity. 

Scriptable Objects don't get loaded until referenced in the scene. This can cause problems if you're trying to locate an object by script that hasn't been loaded yet. For this reason, and simply for organization, it's often useful to group collections of scriptable objects together in one parent object that can be referenced. This can be done in the editor using AssetDatabase.AddObjectToAsset(), but it can be annoying to have to create an editor class just for this, and even if you do it can be a little tricky. The goal of this library is to supply some simple tools to make adding objects to assets easy.

To use SOCollections just copy or clone the files into your project. There are two ways to use the tool:

### Method 1
The easiest way to use this tool is to simply add a SOCollection variable to any ScriptableObject class that needs it, along with the appropriate using directive (using StellarDoor.SOCollection). This will show up like a list in the inspector. 

*image

You can add any object that derives from ScriptableObject to this list by either dragging them onto the list title, or by clicking the '+' button and adding one to the empty element field that it creates. 

You'll notice that when you do this the scriptable object asset file of the object will move to be a child object of the one that contains the list. You can verify this by clicking the triangle next to its name to unfold it.

*image

_What actually happens is that an exact copy of the asset is added to the object, and the original is deleted._ While the added object will have all the data of the original, any references to it in other places, e.g. MonoBehaviours that reference it in the scene will lose the reference. Be aware that you'll have to reassign the object in the inspector where needed. You can do this the same way you would any scriptable object, by dragging it or by using the object picker to locate it.

You can remove the objects by clicking the "X" next to that item. This will move the object back out of the containing object. Behind the scenes it deletes the child object and creates a copy of it in the same folder as the object that was referencing it.

You can also replace an object that's already in the list, either by dragging or by using the object picker. If you do so the replaced item gets copied to a stand alone asset file, while the added item gets copied into the object.

### Features and Limitations

You can add more than one SOCollection to the same object.

You _can_ add an object that contains a SOCollection as a child as long as the child collection is empty when you add it. You can then add objects to the SOCollection on the child object. _Note that the added objects will appear attached to the parent object._ Unlike objects in the hierarchy, only a single child layer is allowed on any scriptable object. The references to these objects are still stored in the child SOCollection though, so in code it shouldn't matter.

You _can't_ add an object that already has child objects to the collection -- this will destroy the child objects. If you need to, first remove the child objects then re-add after you attach their parent.

The SOCollection functionality is contained in two classes: SOCollection.cs and editor class SOCollectionDrawer.cs. If that's all you need, you can remove everything else.

### Method 2

The second method is a little more complex, but can be a more powerful way to create and manage groups of scriptable objects of the same type.

*todo


