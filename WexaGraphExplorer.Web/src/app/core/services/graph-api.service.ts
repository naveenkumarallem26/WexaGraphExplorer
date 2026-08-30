import {
  Injectable,
  inject
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable,
  catchError,
  map,
  throwError
} from 'rxjs';

import {
  GraphSummary,
  MissingTalent,
  ProjectDependency
} from '../models/graph.models';

interface ApiListResponse<T> {
  value: T[];
  count: number;
}

type ApiArrayResponse<T> =
  T[] | ApiListResponse<T>;

@Injectable({
  providedIn: 'root'
})
export class GraphApiService {

  private readonly http =
    inject(HttpClient);

  private readonly apiUrl =
    'https://wexa-graph-api.onrender.com/api/graph';

  getSummary(): Observable<GraphSummary[]> {

    return this.http
      .get<ApiArrayResponse<GraphSummary>>(
        `${ this.apiUrl }/summary`
      )
      .pipe(
  map(response =>
    Array.isArray(response)
      ? response
      : response.value
  ),

  catchError(error => {

    console.error(
      'Graph summary request failed:',
      error
    );

    return throwError(
      () =>
        new Error(
          'Unable to load graph summary. Make sure the API is running.'
        )
    );
  })
);
  }

getMissingTalent(
  projectName: string
): Observable < MissingTalent[] > {

  const encodedProject =
    encodeURIComponent(projectName);

  return this.http
    .get<ApiArrayResponse<MissingTalent>>(
      `${this.apiUrl}/projects/${encodedProject}/missing-talent`
    )
    .pipe(
      map(response =>
        Array.isArray(response)
          ? response
          : response.value
      ),

      catchError(error => {

        console.error(
          'Missing talent request failed:',
          error
        );

        return throwError(
          () =>
            new Error(
              'Unable to load missing talent data. Check the project name and API connection.'
            )
        );
      })
    );
}

getDependencies(
  projectName: string
): Observable < ProjectDependency[] > {

  const encodedProject =
    encodeURIComponent(projectName);

  return this.http
    .get<ApiArrayResponse<ProjectDependency>>(
      `${this.apiUrl}/projects/${encodedProject}/dependencies`
    )
    .pipe(
      map(response =>
        Array.isArray(response)
          ? response
          : response.value
      ),

      catchError(error => {

        console.error(
          'Dependency request failed:',
          error
        );

        return throwError(
          () =>
            new Error(
              'Unable to load project dependencies. Check the project name and API connection.'
            )
        );
      })
    );
}
}
