export interface GraphSummary {
  label: string;
  count: number;
}

export interface MissingTalent {
  developerId: string;
  developerName: string;
  company: string | null;
  matchingSkills: string[];
  workCount: number;
}

export interface ProjectDependency {
  connectedProject: string;
  sharedDeveloper: string;
  dependencyChain: string[];
}
