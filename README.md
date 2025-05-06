 <h1 align="center">S.A.P.S Documentation page</h1>


Welcome to the S.A.P.S documentation page. This branch houses the repo that hosts our documentation for our project.

Below will tell you how to install the documentation tool, setup the config, get the documentation files and the plans for the future.

<h2>Setting up</h2>

Before you begin you want to make sure that your repo's are setup in a way that makes getting the docs and updating them easier.

1. Clone/Pull down the most recent version of Main.
2. Make a new folder outside of the SAPS repo named `S.A.P.SDocs`
3. Inside that folder clone down the documentation branch with the command:

`git clone --single-branch --branch documentation https://github.com/OtagoPolytechnic/S.A.P.S.git`

You are now ready to start documenting

<h2>Installing Doxygen</h2>
Doxygen is the tool we use to generate our documentation. It was made originally for C++ but has functionality for C#.

1. Go to the [Doxygen download page](https://doxygen.nl/download.html) and install the version for your OS.
2. After installing and setup run the `doxywizard.exe` file

<h2>Setting the config with included config file</h2>

Before you continue you need to load the config file given to make your life easier.

Simply go File, Open and open the file called Doxyfile.

![image](https://github.com/user-attachments/assets/3635cb0a-0ccb-40f6-8b1b-ffa88f06d24a)

You should have all the relevant settings set and all you need to do is change a couple as follows:

![image](https://github.com/user-attachments/assets/c2f4b7af-6d2b-467e-accc-16149e92cd3e)

1. Make sure that the working directory is set to the top of the SAPSDocs repo like shown in the picture
2. Make sure that the version is set to the current version of the game
3. Make sure the source code is pointing at scripts folder in the game directory
4. Make sure the destination of the documentation is set to the same place as the working directory
5. Optional: If there is a small version of the logo within the files add it here (may already be there)

After completing those steps scroll down to Generating the files to procede.

<h2>Setting the config without the included config file</h2>
 
<h3>*Not recommended unless something has gone wrong*</h3>

<h3>Project settings</h3>

At the top there should be a working directory box. Put in /S.A.P.SDocs/S.A.P.S 

1. Change the following settings for the project

![image](https://github.com/user-attachments/assets/20997be0-faea-4b2f-90a1-e29d936c152a)

*Make sure to use the SAPS logo included in this branch*

2. Add the directory for the code you want added to the documentation. It should be /S.A.P.S/Assets/OurFiles/Scripts

![image](https://github.com/user-attachments/assets/c4be6a03-64f8-4f51-aa17-80bd580034b7)

*Make sure scan recursively is ticked on*

3. Add the directory for the documentation to be written to. It should be /S.A.P.SDocs/S.A.P.S

![image](https://github.com/user-attachments/assets/a841ca04-9253-4f4d-b0fe-8728629f87ca)

*The reason it is the top of the SAPS folder and not the /docs folder is because you are going to move files around that Doxygen makes*

**Click the NEXT button to continue**

<h3>Mode settings</h3>

For mode we want to include all entities and make sure we are optimizing for C# (the Java and C# option)

![image](https://github.com/user-attachments/assets/d80033aa-579c-4f76-b0fb-bcd48f25b2b2)

**Click the NEXT button to continue**

<h3>Output Settings</h3>

We want to only generate an HTML file with a navigation panel for our documentation

![image](https://github.com/user-attachments/assets/cdeb3d7d-86d5-4725-88b2-33b44601bbfa)

*Make sure the with search function is ticked on*

**Click the NEXT button to continue**

<h3>Diagram Settings</h3>

You can use the built-in class diagram generator but feel free to use the dot tool from the GraphViz package option if you want

![image](https://github.com/user-attachments/assets/c1bd197a-dad0-4680-8407-ac33a55d551e)

*If you do choose to use the package make sure to tick every option*


**Click the NEXT button to continue**

<h2>Generating the files</h2>

In the run tab you simply need to hit the `Run doxygen` button and it will generate all the files

![image](https://github.com/user-attachments/assets/6661c9cc-107d-4a67-ae04-6a1a8a1532ad)

<h3>Placing the files into the correct area</h3>

Sitting in your SAPSDocs folder should be a file titled `\html`.

Inside is all the files that the documentation generated. You know need to take all the contents of `\html` and place them within the `\docs` folder

HTML Folder

![image](https://github.com/user-attachments/assets/552c75ce-dd37-4c51-b69a-fac20cbc1f74)

DOCS Folder

![image](https://github.com/user-attachments/assets/16d656b6-c07a-461a-a303-7f450a3f6d25)

After that simple commit and push to the branch where the action should run and update the site.

![image](https://github.com/user-attachments/assets/7c72dee3-9ed2-40d1-927b-f78ceb675f7d)

<h2>Future Improvements</h2>

This entire process can be automated as Doxygen has a CMD tool. The process of doing so is a little too out of scope right now but can be done with some patience.

