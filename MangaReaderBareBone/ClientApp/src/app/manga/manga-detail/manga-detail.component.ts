import { HttpParams, HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';

@Component({
  selector: 'app-manga-detail',
  templateUrl: './manga-detail.component.html',
  styleUrls: ['./manga-detail.component.css']
})
export class MangaDetailComponent implements OnInit {
  manga: any;
  allChapters: any;

  constructor(private activatedRoute: ActivatedRoute,
    private http: HttpClient) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
    });
    let params = new HttpParams();
    params = params.append("mangaName", this.manga);
    this.http.get('https://localhost:7235/api/MangaLogs/mangaName/', { responseType: 'text', params: params,  }).pipe(map(r => JSON.parse(r, this.reviver))).subscribe(response => {
      this.allChapters = response;
    }, error => {
      console.log(error);
    })
  }
  reviver(key: string, value: string | number | Date | null): any {
    if (value !== null && (key === 'dateTime'))
      return new Date(value).toLocaleString();

    return value;
  }
}



