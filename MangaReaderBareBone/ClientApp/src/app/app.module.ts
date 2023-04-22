import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { MangaComponent } from './manga/manga.component';
import { MangaDetailComponent } from './manga/manga-detail/manga-detail.component';
import { MangaReadComponent } from './manga/manga-read/manga-read.component';
import { MangaSearchComponent } from './manga/manga-search/manga-search.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

import { ChapterService } from './services/chapter.service';
import { MangaService } from './services/manga.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MangaComponent,
    MangaDetailComponent,
    MangaReadComponent,
    MangaSearchComponent,
    NavMenuComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'Manga', component: MangaComponent },
      { path: 'Manga/:mangaName', component: MangaDetailComponent },
      { path: 'Manga/:mangaName/:chapterName', component: MangaReadComponent },
      { path: 'Search/:keyword', component: MangaSearchComponent },
      { path: '404', component: PageNotFoundComponent },
      { path: '**', redirectTo: '404' }
    ], { scrollPositionRestoration: 'enabled' })
  ],
  providers: [
    ChapterService,
    MangaService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
