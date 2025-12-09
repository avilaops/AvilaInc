import mongoose from 'mongoose';

export interface ICase {
  _id?: string;
  clientName: string;
  clientEmail: string;
  clientPhone?: string;
  clientCompany?: string;
  caseDescription: string;
  caseCategory: string; // Ex: "marketing", "vendas", "operacional"
  objectives?: string[];
  challenges?: string[];
  budget?: number;
  timeline?: string;
  attachments?: string[];
  analysis?: {
    strategy: string;
    recommendations: string[];
    timeline: string;
    estimatedBudget?: number;
    risks?: string[];
  };
  proposal?: {
    htmlContent: string;
    sent: boolean;
    sentAt?: Date;
  };
  status: 'draft' | 'analyzing' | 'analyzed' | 'proposal_sent' | 'completed';
  createdAt: Date;
  updatedAt: Date;
}

const caseSchema = new mongoose.Schema<ICase>(
  {
    clientName: {
      type: String,
      required: true,
    },
    clientEmail: {
      type: String,
      required: true,
    },
    clientPhone: String,
    clientCompany: String,
    caseDescription: {
      type: String,
      required: true,
    },
    caseCategory: {
      type: String,
      required: true,
    },
    objectives: [String],
    challenges: [String],
    budget: Number,
    timeline: String,
    attachments: [String],
    analysis: {
      strategy: String,
      recommendations: [String],
      timeline: String,
      estimatedBudget: Number,
      risks: [String],
    },
    proposal: {
      htmlContent: String,
      sent: {
        type: Boolean,
        default: false,
      },
      sentAt: Date,
    },
    status: {
      type: String,
      enum: ['draft', 'analyzing', 'analyzed', 'proposal_sent', 'completed'],
      default: 'draft',
    },
  },
  { timestamps: true }
);

export const Case = mongoose.model<ICase>('Case', caseSchema);
