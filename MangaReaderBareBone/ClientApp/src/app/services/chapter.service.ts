import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {

  constructor(private http: HttpClient) { }

  getChapterList(mangaName: any) {
    let params = new HttpParams();
    params = params.append("mangaName", mangaName);
    return this.http.get('https://localhost:7235/api/Mangas/chapters', { responseType: 'text', params: params }).pipe(map(res => JSON.parse(res)));
    
  }

  getChapter(mangaName: any, chapterName: any) {
    let params = new HttpParams();
    params = params.append("mangaName", mangaName);
    params = params.append("chapterName", chapterName);
    return this.http.get('https://localhost:7235/api/Mangas/chapters', { responseType: 'text', params: params }).pipe(map(res => JSON.parse(res)));
  }

  postReadChapter(mangaName: any, chapterName: any) {
    let mangaLog = {
      Status: "Read",
      MangaName: mangaName,
      ChapterName: chapterName
    };
    return this.http.post('https://localhost:7235/api/MangaLogs', mangaLog);
  }

}

