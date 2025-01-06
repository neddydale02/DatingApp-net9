import { Component, input, Input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  member = input.required<Member>();

  constructor(private likesService: LikesService) {}

  hasLiked(): boolean {
    return this.likesService.likeIds().includes(this.member().id);
  }

  toggleLike(): void {
    this.likesService.toggleLike(this.member().id).subscribe();
  }
}
