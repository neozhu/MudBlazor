﻿namespace BlazorRepl.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    using BlazorRepl.Core;

    using Microsoft.CodeAnalysis.CSharp;

    public static class CodeFilesHelper
    {
        private const string ValidCodeFileExtension = ".razor";
        private const int MainComponentFileContentMinLength = 10;

        public static string NormalizeCodeFilePath(string path, out string error)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "Path cannot be empty.";
                return null;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!SyntaxFacts.IsValidIdentifier(fileName))
            {
                error = $"'{fileName}' is not a valid file name. It should be a valid C# identifier.";
                return null;
            }

            var extension = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(extension) &&
                !ValidCodeFileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
            {
                error = $"Extension cannot be '{extension}'. Valid extensions: {ValidCodeFileExtension}";
                return null;
            }

            error = null;
            return fileName + ValidCodeFileExtension;
        }

        public static string ValidateCodeFilesForSnippetCreation(IEnumerable<CodeFile> codeFiles)
        {
            if (codeFiles == null)
            {
                throw new ArgumentNullException(nameof(codeFiles));
            }

            var containsMainComponent = false;
            var filePaths = new HashSet<string>();
            var index = 0;
            foreach (var codeFile in codeFiles)
            {
                if (codeFile == null)
                {
                    return $"File #{index} - no file.";
                }

                if (string.IsNullOrWhiteSpace(codeFile.Path))
                {
                    return $"File #{index} - no file path.";
                }

                if (filePaths.Contains(codeFile.Path))
                {
                    return $"File '{codeFile.Path}' is duplicated.";
                }

                // TODO: Reuse with above
                var extension = Path.GetExtension(codeFile.Path);
                if (!ValidCodeFileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return $"File '{codeFile.Path}' has invalid extension: {extension}. Valid extensions: {ValidCodeFileExtension}";
                }

                // TODO: Validate valid identifier (reuse) + main component content >= 10 + etc
                filePaths.Add(codeFile.Path);
            }

            if (!containsMainComponent)
            {
                return "No main component file provided.";
            }

            return null;
        }
    }
}
