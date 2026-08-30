import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { GraphApiService } from '../../core/services/graph-api.service';
import { ProjectDependency } from '../../core/models/graph.models';

@Component({
  selector: 'app-dependencies',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './dependencies.html',
  styleUrl: './dependencies.scss'
})
export class Dependencies {

  private readonly graphApi = inject(GraphApiService);

  projectName = '';

  dependencies: ProjectDependency[] = [];

  loading = false;

  searched = false;

  errorMessage = '';

  search(): void {
    const project = this.projectName.trim();

    if (!project) {
      this.dependencies = [];
      this.searched = false;
      this.errorMessage = 'Please enter a project name.';
      return;
    }

    this.loading = true;
    this.searched = true;
    this.errorMessage = '';
    this.dependencies = [];

    this.graphApi.getDependencies(project).subscribe({
      next: data => {
        this.dependencies = data;
        this.loading = false;
      },

      error: error => {
        this.loading = false;

        this.errorMessage =
          error instanceof Error
            ? error.message
            : 'Unable to load project dependencies.';
      }
    });
  }

  clear(): void {
    this.projectName = '';
    this.dependencies = [];
    this.loading = false;
    this.searched = false;
    this.errorMessage = '';
  }

  retry(): void {
    if (this.projectName.trim()) {
      this.search();
    }
  }
}
