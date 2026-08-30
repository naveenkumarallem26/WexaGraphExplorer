import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard/dashboard')
        .then(m => m.Dashboard)
  },

  {
    path: 'talent-finder',
    loadComponent: () =>
      import('./pages/talent-finder/talent-finder')
        .then(m => m.TalentFinder)
  },

  {
    path: 'dependencies',
    loadComponent: () =>
      import('./pages/dependencies/dependencies')
        .then(m => m.Dependencies)
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
