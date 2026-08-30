// ============================================================
// 1. GRAPH SUMMARY
// ============================================================

MATCH (n)
RETURN
    labels(n)[0] AS Label,
    count(n) AS Count
ORDER BY Label;


// ============================================================
// 2. MULTI-HOP PROJECT DEPENDENCY
// ============================================================

MATCH path =
    (target:Project {
        name: $projectName
    })
    -[:USES_SKILL*1..2]-
    (connectedProject:Project)

WHERE connectedProject <> target

RETURN
    connectedProject.name AS ConnectedProject,

    [
        node IN nodes(path) |
        labels(node)[0] +
        ':' +
        coalesce(node.name, node.email)
    ] AS DependencyChain;


// ============================================================
// 3. DEVELOPERS WITH MATCHING SKILLS
// ============================================================

MATCH (project:Project {
    name: $projectName
})

MATCH (developer:Developer)
    -[:HAS_SKILL]->
    (skill:Skill)

MATCH (project)
    -[:USES_SKILL]->
    (skill)

RETURN
    developer.name AS Developer,
    collect(DISTINCT skill.name) AS MatchingSkills
ORDER BY Developer;