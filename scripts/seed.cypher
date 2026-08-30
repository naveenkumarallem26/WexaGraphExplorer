// ============================================================
// WEXA GRAPH EXPLORER
// COMPLETE DATABASE SEED
// ============================================================
//
// This seed creates a clean, deterministic graph.
//
// IMPORTANT:
// Every statement is independent.
// Do NOT rely on variables created by previous statements.
//
// The C# seeder executes statements separated by ';'.
// Therefore every relationship statement explicitly MATCHes
// the required nodes.
//
// ============================================================


// ============================================================
// 0. CLEAN EXISTING GRAPH
// ============================================================

MATCH (n)
DETACH DELETE n;


// ============================================================
// 1. COMPANIES
// ============================================================

CREATE
    (:Company {
        name: 'Microsoft',
        location: 'Redmond, USA',
        industry: 'Technology'
    }),
    (:Company {
        name: 'Contoso Technologies',
        location: 'Hyderabad, India',
        industry: 'Technology'
    }),
    (:Company {
        name: 'Fabrikam Solutions',
        location: 'Bengaluru, India',
        industry: 'Technology'
    });


// ============================================================
// 2. TECHNOLOGIES
// ============================================================

CREATE
    (:Technology {
        name: '.NET 8',
        category: 'Backend'
    }),
    (:Technology {
        name: 'Angular',
        category: 'Frontend'
    }),
    (:Technology {
        name: 'Microsoft Azure',
        category: 'Cloud'
    }),
    (:Technology {
        name: 'SQL Server',
        category: 'Database'
    }),
    (:Technology {
        name: 'Docker',
        category: 'DevOps'
    });


// ============================================================
// 3. SKILLS
// ============================================================

CREATE
    (:Skill {
        name: 'C#',
        category: 'Backend'
    }),
    (:Skill {
        name: 'ASP.NET Core',
        category: 'Backend'
    }),
    (:Skill {
        name: 'Web API',
        category: 'Backend'
    }),
    (:Skill {
        name: 'Angular',
        category: 'Frontend'
    }),
    (:Skill {
        name: 'TypeScript',
        category: 'Frontend'
    }),
    (:Skill {
        name: 'Azure',
        category: 'Cloud'
    }),
    (:Skill {
        name: 'SQL',
        category: 'Database'
    }),
    (:Skill {
        name: 'Docker',
        category: 'DevOps'
    });


// ============================================================
// 4. DEVELOPERS
// ============================================================

CREATE
    (:Developer {
        email: 'naveen@example.com',
        name: 'Naveen Kumar',
        experienceYears: 4,
        location: 'Hyderabad, India'
    }),
    (:Developer {
        email: 'rahul@example.com',
        name: 'Rahul Sharma',
        experienceYears: 6,
        location: 'Bengaluru, India'
    }),
    (:Developer {
        email: 'priya@example.com',
        name: 'Priya Reddy',
        experienceYears: 5,
        location: 'Hyderabad, India'
    }),
    (:Developer {
        email: 'arjun@example.com',
        name: 'Arjun Mehta',
        experienceYears: 3,
        location: 'Pune, India'
    });


// ============================================================
// 5. PROJECTS
// ============================================================

CREATE
    (:Project {
        name: 'Employee Management Portal',
        description: 'Enterprise employee management application'
    }),
    (:Project {
        name: 'Digital Banking Platform',
        description: 'Cloud-based digital banking application'
    }),
    (:Project {
        name: 'E-Commerce Platform',
        description: 'Scalable online commerce platform'
    }),
    (:Project {
        name: 'Healthcare Management System',
        description: 'Healthcare appointment and patient management system'
    });


// ============================================================
// 6. DEVELOPER -> SKILL
// ============================================================

// Naveen Kumar
MATCH
    (d:Developer {name: 'Naveen Kumar'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'ASP.NET Core'}),
    (s3:Skill {name: 'Web API'}),
    (s4:Skill {name: 'Angular'}),
    (s5:Skill {name: 'Azure'}),
    (s6:Skill {name: 'SQL'})
CREATE
    (d)-[:HAS_SKILL]->(s1),
    (d)-[:HAS_SKILL]->(s2),
    (d)-[:HAS_SKILL]->(s3),
    (d)-[:HAS_SKILL]->(s4),
    (d)-[:HAS_SKILL]->(s5),
    (d)-[:HAS_SKILL]->(s6);


// Rahul Sharma
MATCH
    (d:Developer {name: 'Rahul Sharma'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'ASP.NET Core'}),
    (s3:Skill {name: 'Web API'}),
    (s4:Skill {name: 'Azure'}),
    (s5:Skill {name: 'Docker'})
CREATE
    (d)-[:HAS_SKILL]->(s1),
    (d)-[:HAS_SKILL]->(s2),
    (d)-[:HAS_SKILL]->(s3),
    (d)-[:HAS_SKILL]->(s4),
    (d)-[:HAS_SKILL]->(s5);


// Priya Reddy
MATCH
    (d:Developer {name: 'Priya Reddy'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'Angular'}),
    (s3:Skill {name: 'TypeScript'}),
    (s4:Skill {name: 'SQL'})
CREATE
    (d)-[:HAS_SKILL]->(s1),
    (d)-[:HAS_SKILL]->(s2),
    (d)-[:HAS_SKILL]->(s3),
    (d)-[:HAS_SKILL]->(s4);


// Arjun Mehta
MATCH
    (d:Developer {name: 'Arjun Mehta'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'Web API'}),
    (s3:Skill {name: 'Docker'})
CREATE
    (d)-[:HAS_SKILL]->(s1),
    (d)-[:HAS_SKILL]->(s2),
    (d)-[:HAS_SKILL]->(s3);


// ============================================================
// 7. DEVELOPER -> COMPANY
// ============================================================

MATCH
    (d:Developer {name: 'Naveen Kumar'}),
    (c:Company {name: 'Contoso Technologies'})
CREATE
    (d)-[:WORKED_AT]->(c);


MATCH
    (d:Developer {name: 'Rahul Sharma'}),
    (c:Company {name: 'Microsoft'})
CREATE
    (d)-[:WORKED_AT]->(c);


MATCH
    (d:Developer {name: 'Priya Reddy'}),
    (c:Company {name: 'Fabrikam Solutions'})
CREATE
    (d)-[:WORKED_AT]->(c);


MATCH
    (d:Developer {name: 'Arjun Mehta'}),
    (c:Company {name: 'Contoso Technologies'})
CREATE
    (d)-[:WORKED_AT]->(c);


// ============================================================
// 8. DEVELOPER -> PROJECT
// ============================================================

// Naveen
MATCH
    (d:Developer {name: 'Naveen Kumar'}),
    (p:Project {name: 'Employee Management Portal'})
CREATE
    (d)-[:WORKED_ON]->(p);


MATCH
    (d:Developer {name: 'Naveen Kumar'}),
    (p:Project {name: 'E-Commerce Platform'})
CREATE
    (d)-[:WORKED_ON]->(p);


// Rahul
MATCH
    (d:Developer {name: 'Rahul Sharma'}),
    (p:Project {name: 'Digital Banking Platform'})
CREATE
    (d)-[:WORKED_ON]->(p);


MATCH
    (d:Developer {name: 'Rahul Sharma'}),
    (p:Project {name: 'Employee Management Portal'})
CREATE
    (d)-[:WORKED_ON]->(p);


// Priya
MATCH
    (d:Developer {name: 'Priya Reddy'}),
    (p:Project {name: 'E-Commerce Platform'})
CREATE
    (d)-[:WORKED_ON]->(p);


MATCH
    (d:Developer {name: 'Priya Reddy'}),
    (p:Project {name: 'Healthcare Management System'})
CREATE
    (d)-[:WORKED_ON]->(p);


// Arjun
MATCH
    (d:Developer {name: 'Arjun Mehta'}),
    (p:Project {name: 'Digital Banking Platform'})
CREATE
    (d)-[:WORKED_ON]->(p);


// ============================================================
// 9. PROJECT -> SKILL
// ============================================================

// Employee Management Portal
MATCH
    (p:Project {name: 'Employee Management Portal'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'ASP.NET Core'}),
    (s3:Skill {name: 'Web API'}),
    (s4:Skill {name: 'Angular'}),
    (s5:Skill {name: 'SQL'})
CREATE
    (p)-[:USES_SKILL]->(s1),
    (p)-[:USES_SKILL]->(s2),
    (p)-[:USES_SKILL]->(s3),
    (p)-[:USES_SKILL]->(s4),
    (p)-[:USES_SKILL]->(s5);


// Digital Banking Platform
MATCH
    (p:Project {name: 'Digital Banking Platform'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'ASP.NET Core'}),
    (s3:Skill {name: 'Web API'}),
    (s4:Skill {name: 'Azure'})
CREATE
    (p)-[:USES_SKILL]->(s1),
    (p)-[:USES_SKILL]->(s2),
    (p)-[:USES_SKILL]->(s3),
    (p)-[:USES_SKILL]->(s4);


// E-Commerce Platform
MATCH
    (p:Project {name: 'E-Commerce Platform'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'Angular'}),
    (s3:Skill {name: 'TypeScript'}),
    (s4:Skill {name: 'SQL'})
CREATE
    (p)-[:USES_SKILL]->(s1),
    (p)-[:USES_SKILL]->(s2),
    (p)-[:USES_SKILL]->(s3),
    (p)-[:USES_SKILL]->(s4);


// Healthcare Management System
MATCH
    (p:Project {name: 'Healthcare Management System'}),
    (s1:Skill {name: 'C#'}),
    (s2:Skill {name: 'Web API'}),
    (s3:Skill {name: 'SQL'})
CREATE
    (p)-[:USES_SKILL]->(s1),
    (p)-[:USES_SKILL]->(s2),
    (p)-[:USES_SKILL]->(s3);


// ============================================================
// 10. PROJECT -> TECHNOLOGY
// ============================================================

// Employee Management Portal
MATCH
    (p:Project {name: 'Employee Management Portal'}),
    (t1:Technology {name: '.NET 8'}),
    (t2:Technology {name: 'Angular'}),
    (t3:Technology {name: 'SQL Server'})
CREATE
    (p)-[:USES_TECHNOLOGY]->(t1),
    (p)-[:USES_TECHNOLOGY]->(t2),
    (p)-[:USES_TECHNOLOGY]->(t3);


// Digital Banking Platform
MATCH
    (p:Project {name: 'Digital Banking Platform'}),
    (t1:Technology {name: '.NET 8'}),
    (t2:Technology {name: 'Microsoft Azure'}),
    (t3:Technology {name: 'Docker'})
CREATE
    (p)-[:USES_TECHNOLOGY]->(t1),
    (p)-[:USES_TECHNOLOGY]->(t2),
    (p)-[:USES_TECHNOLOGY]->(t3);


// E-Commerce Platform
MATCH
    (p:Project {name: 'E-Commerce Platform'}),
    (t1:Technology {name: '.NET 8'}),
    (t2:Technology {name: 'Angular'}),
    (t3:Technology {name: 'SQL Server'})
CREATE
    (p)-[:USES_TECHNOLOGY]->(t1),
    (p)-[:USES_TECHNOLOGY]->(t2),
    (p)-[:USES_TECHNOLOGY]->(t3);


// Healthcare Management System
MATCH
    (p:Project {name: 'Healthcare Management System'}),
    (t1:Technology {name: '.NET 8'}),
    (t2:Technology {name: 'Microsoft Azure'}),
    (t3:Technology {name: 'SQL Server'})
CREATE
    (p)-[:USES_TECHNOLOGY]->(t1),
    (p)-[:USES_TECHNOLOGY]->(t2),
    (p)-[:USES_TECHNOLOGY]->(t3);