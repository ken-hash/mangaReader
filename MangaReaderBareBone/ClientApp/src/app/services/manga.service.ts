import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MangaService {

  constructor(private http: HttpClient) { }

  getMangaDetails(mangaName: string) {
    let params = new HttpParams();
    params = params.append("mangaName", mangaName);
    return this.http.get('/api/MangaLogs/mangaName/', { responseType: 'text', params: params, }).pipe(map(res => JSON.parse(res, this.reviver)))
  }

  getMangaList(max: number = 20) {
    let params = new HttpParams();
    params = params.append("max", max);
    return this.http.get('/api/Mangas/mangaList/', { responseType: 'text', params: params, }).pipe(map(res => JSON.parse(res)))
  }

  getLatestChapters(numDays: number = 7, numManga: number = 10) {
    let params = new HttpParams();
    params = params.append("numDays", numDays);
    params = params.append("numManga", numManga);
    return this.http.get('/api/Latest/manga?', { responseType: 'text' }).pipe(map(res => JSON.parse(res)))
  }

  getLastReadMangas(numDays: number = 7, numManga: number = 10) {
    let params = new HttpParams();
    params = params.append("numDays", numDays);
    params = params.append("numManga", numManga);
    return this.http.get('/api/Latest/mangaRead?', { responseType: 'text' }).pipe(map(res => JSON.parse(res)))
  }

  reviver(key: string, value: string | number | Date | null): any {
    let elapsed = undefined;
    if (value !== null && (key === 'dateTime')) {
      elapsed = new Date().getTime() - new Date(value).getTime();
      if (Math.trunc(elapsed / (1000 * 60 * 60 * 24 * 7 * 4)) > 0) {
        return (elapsed / (1000 * 60 * 60 * 24 * 7* 4)).toFixed(0) + " months ago";
      }
      if (Math.trunc(elapsed / (1000 * 60 * 60 * 24)) > 7) {
        return (elapsed / (1000 * 60 * 60 * 24 * 7)).toFixed(0) + " weeks ago";
      }
      else if (Math.trunc(elapsed/(1000 * 60 * 60 * 24)) > 0) {
        return (elapsed / (1000 * 60 * 60 * 24)).toFixed(0) + " days ago";
      }
      else {
        return (elapsed / (1000 * 60 * 60)).toFixed(1) + " hours ago";
      }
    }
    return value;
  }


}
