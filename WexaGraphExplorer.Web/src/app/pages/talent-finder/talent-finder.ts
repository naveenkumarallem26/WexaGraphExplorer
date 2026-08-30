import {
  Component,
  inject
} from '@angular/core';

import {
  FormsModule
} from '@angular/forms';

import {
  GraphApiService
} from '../../core/services/graph-api.service';

import {
  MissingTalent
} from '../../core/models/graph.models';

@Component({
  selector: 'app-talent-finder',

  imports: [
    FormsModule
  ],

  templateUrl: './talent-finder.html',

  styleUrl: './talent-finder.scss'
})
export class TalentFinder {

  private readonly graphApi =
    inject(GraphApiService);

  projectName =
    'Employee Management Portal';

  developers: MissingTalent[] = [];

  loading = false;

  searched = false;

  errorMessage = '';

  searchTalent(): void {

    const project =
      this.projectName.trim();

    if (!project) {

      this.errorMessage =
        'Please enter a project name.';

      return;
    }

    this.loading = true;

    this.searched = true;

    this.errorMessage = '';

    this.graphApi
      .getMissingTalent(project)
      .subscribe({

        next: data => {

          this.developers = data;

          this.loading = false;
        },

        error: error => {

          this.developers = [];

          this.errorMessage =
            error.message;

          this.loading = false;
        }
      });
  }

  clearResults(): void {

    this.developers = [];

    this.searched = false;

    this.errorMessage = '';
  }
}
