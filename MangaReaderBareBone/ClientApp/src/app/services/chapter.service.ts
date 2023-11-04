import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { MangaLog } from '../commons/models/mangalog.model';
import { ChapterDetails } from '../commons/models/chapterdetails.model';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {

  constructor(private http: HttpClient) { }

  getChapterList(mangaName: string): Observable<ChapterDetails[]> {
    const params = new HttpParams().set('mangaName', mangaName);
    return this.http.get<ChapterDetails[]>('/api/Mangas/chapters', { params });
  }

  getChapter(mangaName: string, chapterName: string): Observable<ChapterDetails> {
    const params = new HttpParams()
      .set('mangaName', mangaName)
      .set('chapterName', chapterName);

    return this.http.get<ChapterDetails[]>('/api/Mangas/chapters', { params }).pipe(
      map(data => data[0]),
      catchError(err => this.handleError(err))
    );
  }

  postReadChapter(mangaName: string, chapterName: string): Observable<any> {
    const mangaLog = new MangaLog('Read', mangaName, chapterName);
    return this.http.post('/api/MangaLogs', mangaLog);
  }

  private handleError(error: any): Observable<never> {
    console.error(error);
    throw error;
  }
}
