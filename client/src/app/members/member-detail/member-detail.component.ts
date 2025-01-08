import { Component, OnInit, inject, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { RelativeTimePipe } from '../../relative-time/relative-time.pipe';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, DatePipe, RelativeTimePipe, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  private messageService = inject(MessageService);
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  ngOnInit(): void {
    // Sottoscrive i dati del percorso per ottenere il membro
    this.route.data.subscribe({
      next: data=> {
        this.member = data['member'];
        // Mappa le foto del membro nella galleria
        this.member && this.member.photos.map(p => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
        });
      }
    });

    // Sottoscrive i parametri della query per selezionare la scheda
    this.route.queryParams.subscribe({
      next:params=>{
        params['tab'] && this.selectTab(params['tab']);
      }
    })
  }

  // Aggiunge un nuovo messaggio all'array dei messaggi
  onUpdateMessages(event: Message) {
    this.messages.push(event);
  }

  // Seleziona una scheda in base all'intestazione
  selectTab(heading: string){
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(x=> x.heading === heading);
      if (messageTab) messageTab.active = true;
    }
  }

  // Attiva la scheda e carica i messaggi se la scheda attiva è "Messages"
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
  }

  /* Carica il membro in base al nome utente dal percorso
  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member;
        member.photos.map(p => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
        });
      }
    });
  } */
}