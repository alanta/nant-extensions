# Introduction #
TeamCity currently does not support the MbUnit test framework out of the box. This task integrates MbUnit test results into the TeamCity build results.
The task is based on the NAnt task that comes with MbUnit, but extended to check if it is running within a TeamCity build. If so, the task will output test results to TeamCity. The test results will show up in the project statistics.

![http://www.alanta.nl/download/nant-extensions-test-count-chart.png](http://www.alanta.nl/download/nant-extensions-test-count-chart.png)

Detailed test results (stack traces, exception information etc.) are also available.

# Publishing statistics #
The MbUnit task supports some additional statistics that you can easily publish using the [tc-addstatistic-fromprops](http://www.therightstuff.de/projects/nant-extensions/tasks/tc-addstatistic-fromprops.html) task. After running the MbUnit tasks add the following to your NAnt build script:

```
 <tc-addstatistic-fromprops starting-with="${mbunit::get-counter-prefix()}" />
```

You can turn the statistics information into a graph in TeamCity. This can be accomplished by adding the following to the main-config.xml of your TeamCity server:

```
  <graph>
        <valueType key="mbunit.asserts" title="MbUnit Asserts"/>
  </graph>
```

The `key` attribute refers to one of the statistics value published by the `tc-addstatistic-fromprops` task. In this case a graph will display the number of MbUnit asserts.

![http://www.alanta.nl/download/nant-extensions-mbunit-asserts-chart.png](http://www.alanta.nl/download/nant-extensions-mbunit-asserts-chart.png)

Check the documentation for [a list of available statistics](http://www.therightstuff.de/projects/nant-extensions/tasks/mbunit-initcounters.html).

Note that TeamCity 3.x supports only one series per graph. TeamCity 4.x allows multiple series in a single chart.

# Known issues #
Due to the way MbUnit logs it's test results the timing information for the tests shown by TeamCity 3.x is not valid. TeamCity 4.x fully supports timing information.

# Reference #
  * [MbUnit Task Reference](http://www.therightstuff.de/projects/nant-extensions/tasks/mbunit.html)
  * TeamCity 3.x : [Reporting and Displaying Custom Statistics](http://www.jetbrains.net/confluence/display/TCD3/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-stats)
  * TeamCity 4.x : [Custom chart](http://www.jetbrains.net/confluence/display/TCD4/Custom+Chart)