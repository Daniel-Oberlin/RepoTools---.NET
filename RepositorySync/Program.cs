﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RepositoryManifest;
using System.Resources;


namespace RepositorySync
{
    class Program
    {
        static void Main(string[] args)
        {
            int exitCode = 0;
            Utilities.Console console = new Utilities.Console();

            DateTime startTime = DateTime.Now;

            int argIndex = 0;
            string commandArg = "";

            IRepositoryProxy sourceRep = null;
            IRepositoryProxy destRep = null;

            if (argIndex < args.Count())
            {
                commandArg = args[argIndex++];
            }

            if (args.Length >= 3)
            {
                argIndex += 2;
                IRepositoryProxy otherProxy = null;

                // TODO: Handle errors
                try
                {
                    sourceRep = new LocalRepositoryProxy(
                        new System.IO.DirectoryInfo(
                            args[1]));

                    otherProxy = sourceRep;
                }
                catch (Exception)
                {
                    // TODO
                }

                try
                {
                    destRep = new LocalRepositoryProxy(
                        new System.IO.DirectoryInfo(
                            args[2]));

                    otherProxy = destRep;
                }
                catch (Exception)
                {
                    // TODO
                }

                if (sourceRep == null)
                {
                    try
                    {
                        sourceRep = new RemoteRepositoryProxy(
                            args[1],
                            otherProxy.Manifest);

                        otherProxy = sourceRep;
                    }
                    catch (Exception)
                    {
                        // TODO
                    }
                }

                if (destRep == null)
                {
                    try
                    {
                        destRep = new RemoteRepositoryProxy(
                            args[2],
                            otherProxy.Manifest);

                        otherProxy = destRep;
                    }
                    catch (Exception)
                    {
                        // TODO
                    }
                }
            }

            if (sourceRep == null && destRep == null)
            {
                console.WriteLine("Could not resolve a source and destination repository.");
                Environment.Exit(1);
            }
            else if (sourceRep == null)
            {
                console.WriteLine("Could not resolve a source repository.");
                Environment.Exit(1);
            }
            else if (destRep == null)
            {
                console.WriteLine("Could not resolve a destination repository.");
                Environment.Exit(1);
            }

            RepositorySync syncTool =
                new RepositorySync(sourceRep, destRep);

            syncTool.WriteLogDelegate =
                delegate(String message)
                {
                    console.Write(message);
                };

            if (sourceRep != null && destRep != null)
            {
                syncTool.CompareManifests();
            }

            // Initial screen for valid command
            switch (commandArg)
            {
                case "diff":
                case "update":
                case "sync":
                case "mirror":
                case "repair":
                case "help":
                    break;

                default:
                    console.WriteLine("Unrecognized command \"" + commandArg + "\"");
                    exitCode = 1;
                    Environment.Exit(exitCode);
                    break;
            }

            while (argIndex < args.Length)
            {
                string nextArg = args[argIndex++];

                switch (nextArg)
                {
                    case "-silent":
                        console.Silent = true;
                        break;

                    case "-detail":
                        console.Detail = true;
                        break;
                }
            }

            switch (commandArg)
            {
                case "diff":
                    ShowDiff(syncTool, console);
                    break;

                case "update":
                    syncTool.DoUpdate();
                    break;

                case "sync":
                    syncTool.BothWays = true;
                    syncTool.DoUpdate();
                    break;

                case "mirror":
                    syncTool.Mirror = true;
                    syncTool.DoUpdate();
                    break;

                case "repair":
                    console.WriteLine("TODO: repair is unimplemented.");
                    break;

                case "help":
                    console.Write(Properties.Resources.RepositorySyncHelp);
                    break;
            }

            Environment.Exit(exitCode);
        }

        static void ShowDiff(
            RepositorySync syncTool,
            Utilities.Console console)
        {
            bool different = false;

            Manifest sourceMan = syncTool.SourceRep.Manifest;
            Manifest destMan = syncTool.DestRep.Manifest;

            if (sourceMan.Name != destMan.Name)
            {
                console.WriteLine("Manifest names are different.");
                if (console.Detail)
                {
                    console.WriteLine(
                        "   Source name: " +
                        sourceMan.Name);

                    console.WriteLine(
                        "     Dest name: " +
                        destMan.Name);

                    console.WriteLine();
                }
                different = true;
            }

            if (sourceMan.Guid.Equals(destMan.Guid) == false)
            {
                console.WriteLine("Manifest GUIDs are different.");
                if (console.Detail)
                {
                    console.WriteLine(
                        "   Source GUID: " +
                        sourceMan.Guid.ToString());

                    console.WriteLine(
                        "     Dest GUID: " +
                        destMan.Guid.ToString());

                    console.WriteLine();
                }
                different = true;
            }

            if (sourceMan.DefaultHashMethod != destMan.DefaultHashMethod)
            {
                console.WriteLine("Manifest default hash methods are different.");
                if (console.Detail)
                {
                    console.WriteLine(
                        "   Source method: " +
                        sourceMan.DefaultHashMethod);

                    console.WriteLine(
                        "     Dest method: " +
                        destMan.DefaultHashMethod);

                    console.WriteLine();
                }
                different = true;
            }

            if (sourceMan.Description != destMan.Description)
            {
                console.WriteLine("Manifest descriptions are different.");
                if (console.Detail)
                {
                    console.WriteLine("Source description:");
                    console.WriteLine(sourceMan.Description);
                    console.WriteLine();

                    console.WriteLine("Dest description:");
                    console.WriteLine(destMan.Description);
                    console.WriteLine();
                }
                different = true;
            }

            if (sourceMan.InceptionDateUtc != destMan.InceptionDateUtc)
            {
                console.WriteLine("Manifest creation dates are different.");
                if (console.Detail)
                {
                    console.WriteLine(
                        "   Source date: " +
                        sourceMan.InceptionDateUtc.ToLocalTime().ToString());

                    console.WriteLine(
                        "     Dest date: " +
                        destMan.InceptionDateUtc.ToLocalTime().ToString());

                    console.WriteLine();
                }
                different = true;
            }

            // Not sure if this is really useful

            //if (sourceMan.LastUpdateDateUtc != destMan.LastUpdateDateUtc)
            //{
            //    console.WriteLine("Manifest last update dates are different.");
            //    if (console.Detail)
            //    {
            //        console.WriteLine(
            //            "   Source date: " +
            //            sourceMan.LastUpdateDateUtc.ToLocalTime().ToString());

            //        console.WriteLine(
            //            "     Dest date: " +
            //            destMan.LastUpdateDateUtc.ToLocalTime().ToString());

            //        console.WriteLine();
            //    }
            //    different = true;
            //}

            if (sourceMan.ManifestInfoLastModifiedUtc !=
                destMan.ManifestInfoLastModifiedUtc)
            {
                console.WriteLine("Last change to manifest information dates are different.");
                if (console.Detail)
                {
                    console.WriteLine(
                        "   Source date: " +
                        sourceMan.ManifestInfoLastModifiedUtc.ToLocalTime().ToString());

                    console.WriteLine(
                        "     Dest date: " +
                        destMan.ManifestInfoLastModifiedUtc.ToLocalTime().ToString());

                    console.WriteLine();
                }
                different = true;
            }

            bool ignoreListDifferent = false;
            if (sourceMan.IgnoreList.Count != destMan.IgnoreList.Count)
            {
                ignoreListDifferent = true;
            }
            else
            {
                for (int i = 0; i < sourceMan.IgnoreList.Count; i++)
                {
                    if (sourceMan.IgnoreList[i] != destMan.IgnoreList[i])
                    {
                        ignoreListDifferent = true;
                        break;
                    }
                }
            }

            if (ignoreListDifferent)
            {
                console.WriteLine("Manifest ignore lists are different.");
                if (console.Detail)
                {
                    console.WriteLine("Source list:");
                    foreach (string ignoreString in sourceMan.IgnoreList)
                    {
                        console.WriteLine("   " + ignoreString);
                    }
                    console.WriteLine();

                    console.WriteLine("Dest list: ");
                    foreach (string ignoreString in destMan.IgnoreList)
                    {
                        console.WriteLine("   " + ignoreString);
                    }
                    console.WriteLine();
                }
                different = true;
            }

            if (syncTool.SourceOnlyFiles.Count != 0)
            {
                console.WriteLine(
                    syncTool.SourceOnlyFiles.Count.ToString() +
                    " files are only in source.");

                console.DetailFiles(syncTool.SourceOnlyFiles);
                different = true;
            }

            if (syncTool.DestOnlyFiles.Count != 0)
            {
                console.WriteLine(
                    syncTool.DestOnlyFiles.Count.ToString() +
                    " files are only in destination.");

                console.DetailFiles(syncTool.DestOnlyFiles);
                different = true;
            }

            if (syncTool.ChangedFiles.Count != 0)
            {
                console.WriteLine(
                    syncTool.ChangedFiles.Count.ToString() +
                    " files have changed content.");

                console.DetailFiles(syncTool.ChangedFiles.Keys);
                different = true;
            }

            if (syncTool.LastModifiedDateFiles.Count != 0)
            {
                console.WriteLine(
                    syncTool.LastModifiedDateFiles.Count.ToString() +
                    " files have last-modified dates which are different.");

                console.DetailFiles(syncTool.LastModifiedDateFiles.Keys);
                different = true;
            }

            if (syncTool.RegisteredDateFiles.Count != 0)
            {
                console.WriteLine(
                    syncTool.RegisteredDateFiles.Count.ToString() +
                    " files have manifest creation dates which are different.");

                console.DetailFiles(syncTool.RegisteredDateFiles.Keys);
                different = true;
            }

            if (syncTool.MovedFiles.Count > 0)
            {
                console.WriteLine(syncTool.MovedFiles.Count.ToString() + " files were moved.");
                console.DetailFiles(syncTool.MovedFileOrder, syncTool.MovedFiles, true);
                different = true;
            }

            if (different == false)
            {
                console.WriteLine("No differences.");
            }
        }
    }
}
