# Setup #

There are two ways to deploy the NAnt-Extension tasks.

## 1. Extract zip into the NAnt folder ##
The easiest way is to extract the downloaded zip files into the same folder as the NAnt executable. The zip file's directory structure matches the the directory structure expected by NAnt.

## 2. Use 

&lt;loadtasks&gt;

 ##
NAnt also supports loading tasks from the build script directly. For the MbUnit tasks that would look something like this:
```
  <loadtasks assembly="path-to-custom-tasks/NAntExtensions.MbUnit.dll" />
```

In order for this to work NAnt needs to be able to find all dependencies for the tasks. To accomplish this you should extract all the files from the downloaded zip file into the same folder and load the task from there.

# Requirements #

NAnt-Extensions are compiled for NAnt 0.86 beta 1.

# Reference #
  * NAnt : [Loading Custom Extensions](http://nant.sourceforge.net/release/0.86-beta1/help/fundamentals/tasks.html#taskloader)
  * NAnt : [&lt;loadtasks&gt;](http://nant.sourceforge.net/release/0.86-beta1/help/tasks/loadtasks.html)