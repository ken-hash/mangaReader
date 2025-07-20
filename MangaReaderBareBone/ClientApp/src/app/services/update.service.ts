import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UpdateService {

  constructor(private http: HttpClient) { }

  getUpdate(siteName: string): Observable<Date> {
    const params = new HttpParams().set('site', siteName);

    return this.http.get<string>('api/Updates/site', { params }).pipe(
      map(response => new Date(response)),  // Convert the string response to a Date object
      catchError(this.handleError)
    );
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred:', error);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
