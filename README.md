# FIM Synchronization Testing Tool

Tool designed to facilitate tests of solutions developed for FIM Synchronization service allowing comparisons of directory services content (AD DS and / or AD LDS) and punctual modifications of attributes. This tool is a good complement to FIM product but it is not limited to it.  

This is a grid type application configurable with an XSLX file and with extensions in C#. It provides in one view a list of objects (users, contacts, groups,...) contained in a list of AD sources and FIM database sources.

Objects from different sources are related by attributes identified as "Link" and treated as a unique object. When an object is selected, all configured attributes for each source are shown in one grid. 

Functionalities of grid view:
- Attribute values of all AD sources are directly editable in grid view
- Multi-value attributes are supported
- Image attributes are readable
- Easy comparison of attribute values of a source with the rest of sources

Synchronization Run Profiles of one or more FIM servers (six maximum) can be executed from application menu.

Application can be extended with C# extensions allowing object creation in different AD sources. An additional custom action is allowed too.

New functionality could be added with macro files written in C# and executed from application with CS Script.

## License
- GNU General Public License Version 3.
- FIMSyncTest uses [CS Script](http://www.csscript.net/) engine to execute scripts.
- FIMSyncTest uses [DocumentFormat.OpenXml](https://www.nuget.org/packages/DocumentFormat.OpenXml/) to read configuration.

## Related products
- FIM 2010 Synchronization Service is part of Microsoft Forefront Identity Manager (FIM) 2010 suite.
- Windows Management Instrumentation (WMI) is Microsoft’s implementation of Web-Based Enterprise Management (WBEM).
- Active Directory (AD DS) is a directory service that Microsoft developed for Windows domain networks.
- Active Directory Lightweight Directory Services (AD LDS), formerly known as Active Directory Application Mode (ADAM), is a light-weight implementation of AD DS.







