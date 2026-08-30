import {
  isPlatformBrowser
} from '@angular/common';

import {
  Component,
  PLATFORM_ID,
  inject
} from '@angular/core';

import {
  RouterLink
} from '@angular/router';

import {
  GraphApiService
} from '../../core/services/graph-api.service';

import {
  GraphSummary
} from '../../core/models/graph.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,

  imports: [
    RouterLink
  ],

  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {

  private readonly graphApi =
    inject(GraphApiService);

  private readonly platformId =
    inject(PLATFORM_ID);

  summary: GraphSummary[] = [];

  loading = false;

  errorMessage = '';

  constructor() {

    if (
      isPlatformBrowser(
        this.platformId
      )
    ) {
      this.loadSummary();
    }
  }

  loadSummary(): void {

    this.loading = true;
    this.errorMessage = '';

    this.graphApi
      .getSummary()
      .subscribe({

        next: data => {

          this.summary = data;

          this.loading = false;
        },

        error: error => {

          console.error(error);

          this.summary = [];

          this.errorMessage =
            error instanceof Error
              ? error.message
              : 'Unable to connect to the graph API.';

          this.loading = false;
        }

      });
  }

  getCount(label: string): number {

    const item =
      this.summary.find(
        summary =>
          summary.label.toLowerCase() ===
          label.toLowerCase()
      );

    return item
      ? item.count
      : 0;
  }
}
