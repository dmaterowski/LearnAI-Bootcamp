**Custom Vision API C\# Tutorial**
==================================

The goal of this tutorial is to explore a basic Windows application that uses
the Custom Vision API to create a project, add tags to it, upload images,
train the project, obtain the default prediction endpoint URL for the project,
and use the endpoint to programmatically test an image. You can use this open
source example as a template for building your own app for Windows using the
Custom Vision API.  

**Prerequisites**
-----------------

### Platform requirements

This example has been tested using the .NET Framework using [Visual Studio 2017,
Community Edition](https://www.visualstudio.com/downloads/)


### The Training API key

You also need to have a training API key. The training API key allows you to
create, manage, and train Custom Vision projects programatically. All operations
on <https://customvision.ai> are exposed through this library, allowing you to
automate all aspects of the Custom Vision Service. You can obtain a key by
creating a new project at <https://customvision.ai> and then clicking on the
"setting" gear in the top right.

> Note: Internet Explorer is not supported. We recommend using Edge, Firefox, or Chrome.

### The Images used for Training and Predicting

In the Resources\Images folder are three folders:

- Hemlock
- Japanese Cherry
- Test

The Hemlock and Japenese Cherry folders contain images of these types of plants that
will be trained and tagged. The Test folder contains an image that will be used to 
perform the test prediction


**Lab: Creating a Custom Vision Application**
---------------------------------------------

### Step 1: Create a console application and prepare the training key and the images needed for the example.

 

Start Visual Studio 2017, Community Edition, open the Visual Studio solution
named **CustomVision.Sample.sln** in the sub-directory of where this lab is
located:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Resources/Starter/CustomVision.Sample/CustomVision.Sample.sln
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

This code defines and calls one helper method called `LoadImagesFromDisk` which loads two sets of images that this example uses to train the project, and one test image that the example loads to demonstrate the use of the default
prediction endpoint. 

### Step 2: Create a Custom Vision Service project

To create a new Custom Vision Service project, add your code in the
body of the `Main()` method after the call to `new TrainingApi().`

What method should you replace the _ with to create a new Custom Vision Service project?

```
Project project = null; //TODO#1: Create new project

```

 

### Step 3: Add tags to your project

Implement the following part with creation of the tags.

```
//TODO#2: Make two tags in the new project
Tag hemlockTag = null; //Create Tag "Hemlock";
Tag japaneseCherryTag = null; //Create Tag "Japanese Cherry"
```
 

### Step 4: Upload images to the project

Upload the images to the project. Remember about using proper tags for each image category.
You can either upload images one by one (TODO#3) or in batches (TODO#4).
```
//TODO#3: Images can be uploaded one at a time  
foreach (var image in hemlockImages)
{
    //Add image to a project with a specific tag
}
```

```
 //TODO#4: Or uploaded in a single batch   
ImageFileCreateBatch batch = null; //create a batch
trainingApi.CreateImagesFromFilesWithHttpMessagesAsync(project.Id,batch).Wait();

```

### Step 5: Train the project

What method should you replace the _ with to train the project? 
```
//TODO#5: replace '_' below with a call to training method
var iteration = trainingApi._(project.Id);
```
### Step 6: Get and use the default prediction endpoint

We are now ready to use the model for prediction. First we obtain the endpoint
associated with the default iteration. Then we send a test image to the project using that endpoint. 

### Step 6: Get predicted tag and its probability and print it

Implmement TODO#6:
```
// Loop over each prediction and write out the results  
foreach (var c in result.Predictions)
{
    //TODO#6: Display predicted tag and its probability in Console
}
```

### Step 7: Run the example

Build and run the solution. You will be required to input your training API key
into the console app when running the solution so have this at the ready. The
training and prediction of the images can take 2 minutes. The prediction results
appear on the console.

### Need help?

Start Visual Studio 2017, Community Edition, open the Visual Studio solution
named **CustomVision.Sample.sln** in the solution sub-directory of where this lab is
located:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Resources/Solution/CustomVision.Sample/CustomVision.Sample.sln
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Further Reading
---------------

The source code for the Windows client library is available on
[github](https://github.com/Microsoft/Cognitive-CustomVision-Windows/).

The client library includes multiple sample applications, and this tutorial is
based on the `CustomVision.Sample` demo within that repository.
