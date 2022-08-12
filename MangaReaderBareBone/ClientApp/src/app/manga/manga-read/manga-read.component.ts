import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-manga-read',
  templateUrl: './manga-read.component.html',
  styleUrls: ['./manga-read.component.css']
})
export class MangaReadComponent implements OnInit {
  manga: any;
  chapterName: any;
  chapterDetails: any;
  constructor(private activatedRoute: ActivatedRoute,
    private http: HttpClient  ) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.manga = params['mangaName'];
      this.chapterName = params['chapterName'];
    });
    let params = new HttpParams();
    params = params.append("mangaName", this.manga);
    params = params.append("chapterName", this.chapterName);
    this.http.get('https://localhost:7235/api/Mangas/chapters', { params: params }).subscribe(response => {
        this.chapterDetails = response;
      }, error => {
        console.log(error);
    })
  }
}
