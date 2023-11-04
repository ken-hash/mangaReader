import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MangaService {

  constructor(private http: HttpClient) { }

  private createHttpParams(params: any = {}): HttpParams {
    let httpParams = new HttpParams();
    for (const key in params) {
      if (params[key] !== undefined) {
        httpParams = httpParams.append(key, params[key].toString());
      }
    }
    return httpParams;
  }

  private getWithParams(endpoint: string, params: any = {}): any {
    return this.http.get(endpoint, {
      responseType: 'text',
      params: this.createHttpParams(params),
    }).pipe(map(res => JSON.parse(res, this.reviver)));
  }

  getMangaDetails(mangaName: string) {
    return this.getWithParams('/api/MangaLogs/mangaName', { mangaName });
  }

  getMangaList(max: number = 20) {
    return this.getWithParams('/api/Mangas/mangaList', { max });
  }

  getLatestChapters(numDays: number = 7, numManga: number = 20) {
    return this.getWithParams('/api/Latest/manga', { numDays, numManga });
  }

  getLastReadMangas(numDays: number = 7, numManga: number = 20) {
    return this.getWithParams('/api/Latest/mangaRead', { numDays, numManga });
  }

  searchKeyword(keyword: string) {
    return this.getWithParams('/api/Mangas/Search', { name: keyword });
  }

  reviver(key: string, value: string | number | Date | null): any {
    if (value !== null && key === 'dateTime') {
      const elapsed = new Date().getTime() - new Date(value).getTime();
      const seconds = Math.trunc(elapsed / 1000);
      const minutes = Math.trunc(seconds / 60);
      const hours = Math.trunc(minutes / 60);
      const days = Math.trunc(hours / 24);
      const weeks = Math.trunc(days / 7);
      const months = Math.trunc(weeks / 4);

      if (months > 0) {
        return `${months} months ago`;
      } else if (weeks > 0) {
        return `${weeks} weeks ago`;
      } else if (days > 0) {
        return `${days} days ago`;
      } else if (hours > 0) {
        const remainingMinutes = minutes % 60;
        return `${hours} hours and ${remainingMinutes} minutes ago`;
      } else {
        const remainingSeconds = seconds % 60;
        return `${minutes} minutes and ${remainingSeconds} seconds ago`;
      }
    }
    return value;
  }
}
