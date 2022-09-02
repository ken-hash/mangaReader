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
    if (value !== null && (key === 'dateTime'))
      return new Date(value).toLocaleString();
    return value;
  }


}
