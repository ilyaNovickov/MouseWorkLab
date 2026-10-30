using System;

namespace MouseLab.Core.Services;

public interface IMoveFinderManager
{
    int Resolution { get; set; }

    int PatchSize { get; set; }

    int SearchRange { get; set; }

    bool RelateSearchWithPatch { get; set; }

    IMoveFinder SelectedFinder { get; }
}
