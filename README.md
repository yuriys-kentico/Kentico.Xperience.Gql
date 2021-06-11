# Kentico Xperience Gql Backend

This repository contains the assemblies and a sample server required to host GraphQL endpoints for Kentico Xperience 13.

The GraphQL server acts as a web farm server. This server runs using Hot Chocolate, which is part of the ChilliCream suite of .NET GraphQL tools.

While the GraphQL endpoints can be used with any frontend, they were tested with Gatsby. When Gatsby builds, it queries this server for the GraphQL schema and runs any GraphQL queries against it that it needs to complete building the site. The server also includes a way to refresh the Gatsby site using Gatsby’s refresh endpoint whenever any Kentico Xperience 13 content changes.
