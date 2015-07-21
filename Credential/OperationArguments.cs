﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Helpers.Authentication
{
    internal sealed class OperationArguments
    {
        internal OperationArguments(TextReader stdin)
        {
            Debug.Assert(stdin != null, "The stdin parameter is null");


            this.Authority = AuthorityType.Auto;
            this.Interactivity = Interactivity.Auto;
            this.ValidateCredentials = true;

            string line;
            while (!String.IsNullOrWhiteSpace((line = Console.In.ReadLine())))
            {
                string[] pair = line.Split(new[] { '=' }, 2);

                if (pair.Length == 2)
                {
                    switch (pair[0])
                    {
                        case "protocol":
                            this.Protocol = pair[1];
                            break;

                        case "host":
                            this.Host = pair[1];
                            break;

                        case "path":
                            this.Path = pair[1];
                            break;

                        case "username":
                            this.Username = pair[1];
                            break;

                        case "password":
                            this.Password = pair[1];
                            break;
                    }
                }
            }

            if (this.Protocol != null && this.Host != null)
            {
                this.TargetUri = new Uri(String.Format("{0}://{1}", this.Protocol, this.Host), UriKind.Absolute);
            }
        }

        public readonly string Protocol;
        public readonly string Host;
        public readonly string Path;
        public readonly Uri TargetUri;
        public string Username { get; private set; }
        public string Password { get; private set; }
        public AuthorityType Authority { get; set; }
        public Interactivity Interactivity { get; set; }
        public bool ValidateCredentials { get; set; }

        public void SetCredentials(Credential credentials)
        {
            this.Username = credentials.Username;
            this.Password = credentials.Password;
        }

        public void SetScheme(string value)
        {
            if (String.Equals(value, "MSA", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(value, "Microsoft", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(value, "MicrosoftAccount", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(value, "Live", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(value, "LiveConnect", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(value, "LiveID", StringComparison.OrdinalIgnoreCase))
            {
                this.Authority = AuthorityType.MicrosoftAccount;
            }
            else if (String.Equals(value, "AAD", StringComparison.OrdinalIgnoreCase) ||
                     String.Equals(value, "Azure", StringComparison.OrdinalIgnoreCase) ||
                     String.Equals(value, "AzureDirectory", StringComparison.OrdinalIgnoreCase))
            {
                this.Authority = AuthorityType.AzureDirectory;
            }
            else
            {
                this.Authority = AuthorityType.Basic;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("protocol=")
                   .Append(this.Protocol ?? String.Empty)
                   .Append("\n");
            builder.Append("host=")
                   .Append(this.Host ?? String.Empty)
                   .Append("\n");
            if (this.Path != null)
            {
                builder.Append("path=")
                       .Append(this.Path)
                       .Append("\n");
            }
            if (this.Username != null)
            {
                builder.Append("username=")
                       .Append(this.Username)
                       .Append("\n");
            }
            if (this.Password != null)
            {
                builder.Append("password=")
                       .Append(this.Password)
                       .Append("\n");
            }

            return builder.ToString();
        }
    }
}