import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map } from 'rxjs';
import { MangaLog } from '../commons/models/mangalog.model';
import { ChapterDetails } from '../commons/models/chapterdetails.model';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {

  constructor(private http: HttpClient) { }

  getChapterList(mangaName: string) {
    let params = new HttpParams();
    params = params.append("mangaName", mangaName);
    return this.http.get('/api/Mangas/chapters', { responseType: 'text', params: params }).pipe(map(res => JSON.parse(res)));

  }

  getChapter(mangaName: string, chapterName: string) {
    let params = new HttpParams();
    params = params.append("mangaName", mangaName);
    params = params.append("chapterName", chapterName);
    return this.http.get<ChapterDetails[]>('/api/Mangas/chapters', { params: params }).pipe(map(data => { return data[0]; }),
      catchError((err, caught) => {
        console.error(err);
        throw err;
      }));
  }

  postReadChapter(mangaName: string, chapterName: string) {

    let mangaLog = new MangaLog(
      "Read",
      mangaName,
      chapterName
    );
    return this.http.post('/api/MangaLogs', mangaLog);
  }

}

