//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

namespace Latham.Project.Model
{
    public interface IProjectInfoNode<TProjectInfoNode, TParentProjectInfoNode>
        where TProjectInfoNode : class, IProjectInfoNode<TProjectInfoNode, TParentProjectInfoNode>
        where TParentProjectInfoNode : class
    {
        TParentProjectInfoNode? Parent { get; }
        ProjectInfo? Project { get; }
        TProjectInfoNode Evaluate(bool expandPaths);
    }
}
